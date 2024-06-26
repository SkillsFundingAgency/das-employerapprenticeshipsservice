using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;

namespace SFA.DAS.EAS.Account.Api.Client;

public class SecureHttpClient
{
    private readonly IAccountApiConfiguration _configuration;

    public SecureHttpClient(IAccountApiConfiguration configuration)
    {
        _configuration = configuration;
    }

    // so we can mock for testing
    public SecureHttpClient() { }

    public virtual async Task<string> GetAsync(string url)
    {
        var accessToken =
            IsClientCredentialConfiguration(_configuration.ClientId, _configuration.ClientSecret, _configuration.Tenant)
                ? await GetClientCredentialAuthenticationResult(_configuration.ClientId, _configuration.ClientSecret,
                    _configuration.IdentifierUri, _configuration.Tenant)
                : await GetManagedIdentityAuthenticationResult(_configuration.IdentifierUri);

        using var client = new HttpClient();

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await client.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
    
    public virtual async Task SendWithNoResult(HttpRequestMessage httpRequest)
    {
        var accessToken =
            IsClientCredentialConfiguration(_configuration.ClientId, _configuration.ClientSecret, _configuration.Tenant)
                ? await GetClientCredentialAuthenticationResult(_configuration.ClientId, _configuration.ClientSecret,
                    _configuration.IdentifierUri, _configuration.Tenant)
                : await GetManagedIdentityAuthenticationResult(_configuration.IdentifierUri);

        using var client = new HttpClient();

        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await client.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();
    }

    private static async Task<string> GetClientCredentialAuthenticationResult(string clientId, string clientSecret,
        string resource, string tenant)
    {
        var authority = $"https://login.microsoftonline.com/{tenant}";
        var app = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithAuthority(authority)
            .Build();

        var userAssertion = new UserAssertion(clientSecret);

        var authResult = await app.AcquireTokenOnBehalfOf(
                new[] { $"{resource}/.default" },
                userAssertion
            )
            .ExecuteAsync()
            .ConfigureAwait(false);

        return authResult.AccessToken;
    }

    private static async Task<string> GetManagedIdentityAuthenticationResult(string resource)
    {
        var tokenCredential = new DefaultAzureCredential();
        var accessToken = await tokenCredential.GetTokenAsync(
            new TokenRequestContext(scopes: new[] { resource + "/.default" }) { }
        );
        return accessToken.Token;
    }

    private static bool IsClientCredentialConfiguration(string clientId, string clientSecret, string tenant)
    {
        return !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret) && !string.IsNullOrEmpty(tenant);
    }
}