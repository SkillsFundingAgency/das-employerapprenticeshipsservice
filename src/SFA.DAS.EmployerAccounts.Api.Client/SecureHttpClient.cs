using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    internal class SecureHttpClient : ISecureHttpClient
    {
        private readonly IEmployerAccountsApiClientConfiguration _configuration;

        public SecureHttpClient(IEmployerAccountsApiClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetAsync(string url, CancellationToken cancellationToken = new CancellationToken())
        {
            var baseAddress = new Uri(_configuration.ApiBaseUrl);
            var authenticationResult = await GetAuthenticationResult( _configuration.ClientId, _configuration.ClientSecret, _configuration.IdentifierUri, _configuration.Tenant).ConfigureAwait(false);

            using (var client = new HttpClient { BaseAddress = baseAddress })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);

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
