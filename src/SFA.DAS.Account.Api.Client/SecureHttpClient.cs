using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SFA.DAS.EAS.Account.Api.Client
{
    internal class SecureHttpClient
    {
        private readonly AccountApiConfiguration _configuration;

        public SecureHttpClient(AccountApiConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected SecureHttpClient()
        {
            // So we can mock for testing
        }

        private async Task<AuthenticationResult> GetAuthenticationResult(string clientId, string appKey, string resourceId)
        {
            var authority = "https://login.microsoftonline.com/citizenazuresfabisgov.onmicrosoft.com";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = await context.AcquireTokenAsync(resourceId, clientCredential);
            return result;
        }

        public virtual async Task<string> GetAsync(string url)
        {
            var authenticationResult = await GetAuthenticationResult( _configuration.ClientId,_configuration.ClientSecret, _configuration.IdentifierUri);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
