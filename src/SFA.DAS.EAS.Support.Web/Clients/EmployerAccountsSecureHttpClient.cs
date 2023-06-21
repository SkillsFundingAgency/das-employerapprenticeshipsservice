using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
using SFA.DAS.EmployerAccounts.Api.Client;

namespace SFA.DAS.EAS.Support.Web.Clients;

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
        var accessToken = IsClientCredentialConfiguration()
            ? await GetClientCredentialAuthenticationResult()
            : await GetManagedIdentityAuthenticationResult();

        using var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    private async Task<string> GetClientCredentialAuthenticationResult()
    {
        var authority = $"https://login.microsoftonline.com/{_configuration.Tenant}";
        var app = ConfidentialClientApplicationBuilder.Create(_configuration.ClientId)
            .WithAuthority(authority)
            .Build();

        var userAssertion = new UserAssertion(_configuration.ClientSecret);

        var authResult = await app.AcquireTokenOnBehalfOf(
                new string[] { $"{_configuration.IdentifierUri}/.default" },
                userAssertion
            )
            .ExecuteAsync()
            .ConfigureAwait(false);

        return authResult.AccessToken;
    }

    private async Task<string> GetManagedIdentityAuthenticationResult()
    {
        var tokenCredential = new DefaultAzureCredential();
        var accessToken = await tokenCredential.GetTokenAsync(
            new TokenRequestContext(scopes: new string[] { _configuration.IdentifierUri + "/.default" }) { }
        );
        return accessToken.Token;
    }

    private bool IsClientCredentialConfiguration()
    {
        return !string.IsNullOrEmpty(_configuration.ClientId) && !string.IsNullOrEmpty(_configuration.ClientSecret) && !string.IsNullOrEmpty(_configuration.Tenant);
    }
}
