using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using Microsoft.Identity.Client;
using Azure.Core;
using Azure.Identity;

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
        string text = !IsClientCredentialConfiguration(_configuration.ClientId, _configuration.ClientSecret, _configuration.Tenant) 
            ? await GetManagedIdentityAuthenticationResult(_configuration.IdentifierUri) 
            : await GetClientCredentialAuthenticationResult(_configuration.ClientId, _configuration.ClientSecret, _configuration.IdentifierUri, _configuration.Tenant);
        string parameter = text;
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", parameter);
        HttpResponseMessage obj = await client.GetAsync(url);
        obj.EnsureSuccessStatusCode();
        return await obj.Content.ReadAsStringAsync();
    }

    private async Task<string> GetClientCredentialAuthenticationResult(string clientId, string clientSecret, string resource, string tenant)
    {
        string authority = "https://login.microsoftonline.com/" + tenant;
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

    private async Task<string> GetManagedIdentityAuthenticationResult(string resource)
    {
        var tokenCredential = new DefaultAzureCredential();
        var accessToken = await tokenCredential.GetTokenAsync(
            new TokenRequestContext(scopes: new string[] { resource + "/.default" }) { }
        );
        return accessToken.Token;
    }

    private bool IsClientCredentialConfiguration(string clientId, string clientSecret, string tenant)
    {
        if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
        {
            return !string.IsNullOrEmpty(tenant);
        }

        return false;
    }
}
