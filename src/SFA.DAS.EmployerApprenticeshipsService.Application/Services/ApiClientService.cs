using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using SFA.DAS.EAS.Application.Http;

namespace SFA.DAS.EAS.Application.Services;

public abstract class ApiClientService
{
    protected readonly HttpClient Client;
    private readonly IManagedIdentityTokenGenerator _tokenGenerator;

    protected ApiClientService(HttpClient client, IManagedIdentityTokenGenerator tokenGenerator)
    {
        Client = client;
        _tokenGenerator = tokenGenerator;
    }
    
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

        await AddAuthenticationHeader(request);
        
        var httpResponseMessage = await Client.SendAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            await ThrowRestHttpClientException(httpResponseMessage, cancellationToken);
        }

        return httpResponseMessage;
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
