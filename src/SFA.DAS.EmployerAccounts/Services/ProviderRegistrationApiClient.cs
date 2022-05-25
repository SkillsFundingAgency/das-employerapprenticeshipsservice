using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class ProviderRegistrationApiClient : ApiClientBase, IProviderRegistrationApiClient
    {
        private readonly string _apiBaseUrl;
        private readonly string _identifierUri;
        private readonly HttpClient _client;

        public ProviderRegistrationApiClient(HttpClient client, IProviderRegistrationClientApiConfiguration configuration) : base(client)
        {
            _apiBaseUrl = configuration.BaseUrl.EndsWith("/")
                ? configuration.BaseUrl
                : configuration.BaseUrl + "/";

            _identifierUri = configuration.IdentifierUri;
            _client = client;
        }

        public async Task Unsubscribe(string CorrelationId)
        {
            await AddAuthenticationHeader();
            
            var url = $"{_apiBaseUrl}api/unsubscribe/{CorrelationId}";
            await _client.GetAsync(url);
        }

        public async Task<string> GetInvitations(string CorrelationId)
        {
            await AddAuthenticationHeader();
            
            var url = $"{_apiBaseUrl}api/invitations/{CorrelationId}";
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private async Task AddAuthenticationHeader()
        {
            if (ConfigurationManager.AppSettings["EnvironmentName"].ToUpper() != "LOCAL")
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(_identifierUri);

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
    }
}
