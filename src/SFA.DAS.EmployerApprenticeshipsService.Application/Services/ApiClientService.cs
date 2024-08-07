﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using SFA.DAS.EAS.Application.Http;

namespace SFA.DAS.EAS.Application.Services;

public abstract class ApiClientService
{
    protected readonly HttpClient Client;
    private readonly IManagedIdentityTokenGenerator _tokenGenerator;
    private readonly SemaphoreSlim _tokenLock = new SemaphoreSlim(1);
    private string _accessToken;
    
    protected ApiClientService(HttpClient client, IManagedIdentityTokenGenerator tokenGenerator)
    {
        Client = client;
        _tokenGenerator = tokenGenerator;
    }

    protected async Task PostContent<T>(string uri, T data, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = CreateJsonContent(data);

        await AddAuthenticationHeader(request);

        var httpResponseMessage = await Client.SendAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

        httpResponseMessage.EnsureSuccessStatusCode();
    }
    
    private static HttpContent CreateJsonContent<T>(T data) => data == null ? null : new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
    
    protected Task<TResponse> GetResponse<TResponse>(string uri, object queryData = null, CancellationToken cancellationToken = default)
    {
        return GetInternal<TResponse>(new Uri(uri, UriKind.RelativeOrAbsolute), queryData, cancellationToken);
    }

    private async Task<TResponse> GetInternal<TResponse>(Uri uri, object queryData = null, CancellationToken cancellationToken = default)
    {
        var response = await GetResponse(uri, queryData, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);

        return await response.Content.ReadAsAsync<TResponse>(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
    }

    private async Task<HttpResponseMessage> GetResponse(Uri uri, object queryData = null, CancellationToken cancellationToken = default)
    {
        if (queryData != null)
        {
            uri = new Uri(AddQueryString(uri.ToString(), queryData), UriKind.RelativeOrAbsolute);
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);

        var accessToken = await GetAccessTokenAsync().ConfigureAwait(false);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var httpResponseMessage = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await RefreshAccessTokenAsync().ConfigureAwait(false);
                // Retry the request with the new access token
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                httpResponseMessage = await Client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await ThrowRestHttpClientException(httpResponseMessage, cancellationToken).ConfigureAwait(false);
            }
        }

        return httpResponseMessage;
    }
    
    private async Task<string> GetAccessTokenAsync()
    {
        await _tokenLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                _accessToken = await _tokenGenerator.Generate().ConfigureAwait(false);
            }
            return _accessToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private async Task RefreshAccessTokenAsync()
    {
        await _tokenLock.WaitAsync().ConfigureAwait(false);
        try
        {
            _accessToken = await _tokenGenerator.Generate().ConfigureAwait(false);
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private static string AddQueryString(string uri, object queryData)
    {
        var queryString = queryData
            .GetType()
            .GetProperties()
            .ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(queryData)?.ToString() ?? string.Empty);

        return QueryHelpers.AddQueryString(uri, queryString);
    }

    protected static async Task ThrowRestHttpClientException(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
    {
        var content = await httpResponseMessage.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);

        throw new RestHttpClientException(httpResponseMessage, content);
    }

    protected async Task AddAuthenticationHeader(HttpRequestMessage httpRequestMessage)
    {
        var accessToken = await _tokenGenerator.Generate();
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}