using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net;
using System.Net.Http;

namespace SFA.DAS.EmployerAccounts.Services;

public class HttpService : IHttpService
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _identifierUri;
    private readonly string _tenant;

    public HttpService(string clientId, string clientSecret, string identifierUri, string tenant)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _identifierUri = identifierUri;
        _tenant = tenant;
    }

    public virtual Task<string> GetAsync(string url, bool exceptionOnNotFound = true)
    {
        return GetAsync(url, response => exceptionOnNotFound || response.StatusCode != HttpStatusCode.NotFound);
    }

    public virtual async Task<string> GetAsync(string url, Func<HttpResponseMessage, bool> responseChecker)
    {
        var accessToken = await GetAccessToken();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(url);

            if (responseChecker != null && !responseChecker(response))
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }

    private async Task<string> GetAccessToken()
    {
        var accessToken = IsClientCredentialConfiguration(_clientId, _clientSecret, _tenant)
            ? await GetClientCredentialAuthenticationResult(_clientId, _clientSecret, _identifierUri, _tenant)
            : await GetManagedIdentityAuthenticationResult(_identifierUri);

        return accessToken;
    }

    private static async Task<string> GetClientCredentialAuthenticationResult(string clientId, string clientSecret, string resource, string tenant)
    {
        var authority = $"https://login.microsoftonline.com/{tenant}";
        var clientCredential = new ClientCredential(clientId, clientSecret);
        var context = new AuthenticationContext(authority, true);
        var result = await context.AcquireTokenAsync(resource, clientCredential);
        return result.AccessToken;
    }

    private static async Task<string> GetManagedIdentityAuthenticationResult(string resource)
    {
        var azureServiceTokenProvider = new AzureServiceTokenProvider();
        return await azureServiceTokenProvider.GetAccessTokenAsync(resource);
    }

    private static bool IsClientCredentialConfiguration(string clientId, string clientSecret, string tenant)
    {
        return !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret) && !string.IsNullOrEmpty(tenant);
    }
}