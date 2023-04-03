using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.EmployerAccounts.Api.Client;

namespace SFA.DAS.EAS.Account.Api.Clients;

public class EmployerAccountsSecureHttpClient : ISecureHttpClient
{
    private readonly IEmployerAccountsApiClientConfiguration _configuration;

    public EmployerAccountsSecureHttpClient(IEmployerAccountsApiClientConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected EmployerAccountsSecureHttpClient()
    {
        // so we can mock for testing
    }

    public virtual async Task<string> GetAsync(string url, CancellationToken cancellationToken = default)
    {
        var accessToken = IsClientCredentialConfiguration(_configuration.ClientId, _configuration.ClientSecret, _configuration.Tenant)
            ? await GetClientCredentialAuthenticationResult(_configuration.ClientId, _configuration.ClientSecret, _configuration.IdentifierUri, _configuration.Tenant)
            : await GetManagedIdentityAuthenticationResult(_configuration.IdentifierUri);

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }

    private async Task<string> GetClientCredentialAuthenticationResult(string clientId, string clientSecret, string resource, string tenant)
    {
        var authority = $"https://login.microsoftonline.com/{tenant}";
        var clientCredential = new ClientCredential(clientId, clientSecret);
        var context = new AuthenticationContext(authority, true);
        var result = await context.AcquireTokenAsync(resource, clientCredential);
        return result.AccessToken;
    }

    private async Task<string> GetManagedIdentityAuthenticationResult(string resource)
    {
        var azureServiceTokenProvider = new AzureServiceTokenProvider();
        return await azureServiceTokenProvider.GetAccessTokenAsync(resource);
    }

    private bool IsClientCredentialConfiguration(string clientId, string clientSecret, string tenant)
    {
        return !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret) && !string.IsNullOrEmpty(tenant);
    }
}
