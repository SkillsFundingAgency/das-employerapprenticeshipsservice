using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using SFA.DAS.EAS.Application.Http;

namespace SFA.DAS.EAS.Application.Services;

public abstract class ApiClientService
{
    protected readonly HttpClient _httpClient;

    public ApiClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<T> Get<T>(string uri, object queryData = null, CancellationToken cancellationToken = default)
    {
        return Get<T>(new Uri(uri, UriKind.RelativeOrAbsolute), queryData, cancellationToken);
    }

    public async Task<T> Get<T>(Uri uri, object queryData = null, CancellationToken cancellationToken = default)
    {
        var response = await GetResponse(uri, queryData, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        return await response.Content.ReadAsAsync<T>(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
    }

    private async Task<HttpResponseMessage> GetResponse(Uri uri, object queryData = null, CancellationToken cancellationToken = default)
    {
        if (queryData != null)
        {
            uri = new Uri(AddQueryString(uri.ToString(), queryData), UriKind.RelativeOrAbsolute);
        }

        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            HttpResponseMessage httpResponseMessage2 = httpResponseMessage;
            throw CreateClientException(httpResponseMessage2, await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
        }

        return httpResponseMessage;
    }

    private static string AddQueryString(string uri, object queryData)
    {
        Dictionary<string, string> queryString = queryData.GetType().GetProperties().ToDictionary((PropertyInfo x) => x.Name, (PropertyInfo x) => x.GetValue(queryData)?.ToString() ?? string.Empty);
        return QueryHelpers.AddQueryString(uri, queryString);
    }

    protected virtual Exception CreateClientException(HttpResponseMessage httpResponseMessage, string content)
    {
        return new RestHttpClientException(httpResponseMessage, content);
    }
}
