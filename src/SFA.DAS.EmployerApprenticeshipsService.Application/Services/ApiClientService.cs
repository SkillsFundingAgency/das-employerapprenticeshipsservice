using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.Services;

public abstract class ApiClientService<T> where T : IManagedIdentityClientConfiguration
{
    protected readonly HttpClient Client;
    private readonly IManagedIdentityTokenGenerator<T> _tokenGenerator;

    protected ApiClientService(HttpClient client, IManagedIdentityTokenGenerator<T> tokenGenerator)
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

        await AddAuthenticationHeader();
        
        var httpResponseMessage = await Client.GetAsync(uri, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            var httpResponseMessage2 = httpResponseMessage;
            var restClientException = await CreateClientException(httpResponseMessage2, cancellationToken);
            throw restClientException;
        }

        return httpResponseMessage;
    }

    private static string AddQueryString(string uri, object queryData)
    {
        var queryString = queryData.GetType().GetProperties().ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(queryData)?.ToString() ?? string.Empty);
        return QueryHelpers.AddQueryString(uri, queryString);
    }

    private static async Task<RestHttpClientException> CreateClientException(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
    {
        var content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
        
        return new RestHttpClientException(httpResponseMessage, content);
    }
    
    private async Task AddAuthenticationHeader()
    {
        var accessToken = await _tokenGenerator.Generate();

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }
}
