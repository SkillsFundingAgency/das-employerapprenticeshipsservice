using System.Net.Http;
using System.Net.Http.Headers;
using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;
using SFA.DAS.EAS.Support.Infrastructure.Settings;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

internal class SecureTokenHttpClient: ISecureTokenHttpClient
{
    private readonly ITokenServiceApiClientConfiguration _configuration;

    public SecureTokenHttpClient(ITokenServiceApiClientConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> GetAsync(string url)
    {
        var accessToken = !IsClientCredentialConfiguration(_configuration.ClientId, _configuration.ClientSecret, _configuration.Tenant) 
            ? await GetManagedIdentityAuthenticationResult(_configuration.IdentifierUri) 
            : await GetClientCredentialAuthenticationResult(_configuration.ClientId, _configuration.ClientSecret, _configuration.IdentifierUri, _configuration.Tenant);

        using var client = new HttpClient();

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        using var response = await client.SendAsync(httpRequest);
        
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    private static async Task<string> GetClientCredentialAuthenticationResult(string clientId, string clientSecret, string resource, string tenant)
    {
        var authority = "https://login.microsoftonline.com/" + tenant;
        var app = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithAuthority(authority)
            .Build();

        var userAssertion = new UserAssertion(clientSecret);

        var authResult = await app.AcquireTokenOnBehalfOf(
                new string[] { $"{resource}/.default" },
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
            new TokenRequestContext(scopes: new string[] { resource + "/.default" }) { }
        );
        return accessToken.Token;
    }

    private static bool IsClientCredentialConfiguration(string clientId, string clientSecret, string tenant)
    {
        if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
        {
            return !string.IsNullOrEmpty(tenant);
        }

        return false;
    }
}
