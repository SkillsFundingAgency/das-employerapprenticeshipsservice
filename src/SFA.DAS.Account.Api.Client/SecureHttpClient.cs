﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Identity.Client;
using Azure.Core;

namespace SFA.DAS.EAS.Account.Api.Client
{
    public class SecureHttpClient
    {
        private readonly IAccountApiConfiguration _configuration;

        public SecureHttpClient(IAccountApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected SecureHttpClient()
        {
            // so we can mock for testing
        }

        public virtual async Task<string> GetAsync(string url)
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
            return !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret) && !string.IsNullOrEmpty(tenant);
        }
    }
}
