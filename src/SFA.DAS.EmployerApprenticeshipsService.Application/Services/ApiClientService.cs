using System;
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
    protected readonly HttpClient BaseHttpClient;

    protected ApiClientService(HttpClient baseHttpClient)
    {
        BaseHttpClient = baseHttpClient;
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

        var httpResponseMessage = await BaseHttpClient.GetAsync(uri, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            var httpResponseMessage2 = httpResponseMessage;
            throw CreateClientException(httpResponseMessage2, await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
        }

        return httpResponseMessage;
    }

    private static string AddQueryString(string uri, object queryData)
    {
        var queryString = queryData.GetType().GetProperties().ToDictionary((PropertyInfo x) => x.Name, (PropertyInfo x) => x.GetValue(queryData)?.ToString() ?? string.Empty);
        return QueryHelpers.AddQueryString(uri, queryString);
    }

    private static Exception CreateClientException(HttpResponseMessage httpResponseMessage, string content)
    {
        return new RestHttpClientException(httpResponseMessage, content);
    }
}
