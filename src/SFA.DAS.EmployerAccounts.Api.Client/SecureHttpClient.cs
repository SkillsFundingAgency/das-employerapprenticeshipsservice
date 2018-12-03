﻿using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    internal class SecureHttpClient
    {
        private readonly IEmployerAccountsApiClientConfiguration _configuration;

        public SecureHttpClient(IEmployerAccountsApiClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetAsync(string url)
        {
            var authenticationResult = await GetAuthenticationResult( _configuration.ClientId, _configuration.ClientSecret, _configuration.IdentifierUri, _configuration.Tenant).ConfigureAwait(false);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                var response = await client.GetAsync(url).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        private async Task<AuthenticationResult> GetAuthenticationResult(string clientId, string appKey, string resourceId, string tenant)
        {
            var authority = $"https://login.microsoftonline.com/{tenant}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = await context.AcquireTokenAsync(resourceId, clientCredential).ConfigureAwait(false);

            return result;
        }
    }
}
