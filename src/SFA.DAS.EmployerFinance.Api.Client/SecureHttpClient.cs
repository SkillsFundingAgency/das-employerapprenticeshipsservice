using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;

namespace SFA.DAS.EmployerFinance.Api.Client
{
    public class SecureHttpClient
    {
        private readonly IEmployerFinanceApiClientConfiguration _configuration;

        public SecureHttpClient(IEmployerFinanceApiClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected SecureHttpClient()
        {
            // so we can mock for testing
        }

        public virtual async Task<string> GetAsync(string url, CancellationToken cancellationToken = default)
        {
            var accessToken = await GetManagedIdentityAuthenticationResult(_configuration.IdentifierUri);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        private async Task<string> GetManagedIdentityAuthenticationResult(string resource)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            return await azureServiceTokenProvider.GetAccessTokenAsync(resource);
        }
    }
}
