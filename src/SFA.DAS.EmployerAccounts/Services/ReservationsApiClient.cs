using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Authentication.Extensions.Legacy;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class ReservationsApiClient : ApiClientBase, IReservationsApiClient
    {
        private readonly string ApiBaseUrl;
        private readonly HttpClient _client;
        private readonly string _identifierUri;
        public ReservationsApiClient(HttpClient client, IReservationsClientApiConfiguration configuration) : base(client)
        {
            ApiBaseUrl = configuration.ApiBaseUrl.EndsWith("/")
                ? configuration.ApiBaseUrl
                : configuration.ApiBaseUrl + "/";

            _client = client;
            _identifierUri = configuration.IdentifierUri;
        }

        public async Task<string> Get(long accountId)
        {
             await AddAuthenticationHeader();
            //var uri = $"{ApiBaseUrl}accounts/{accountId}/reservations";
            //var uri = $"{ApiBaseUrl}accounts/1/reservations";
            var uri = $"{ApiBaseUrl}reservation/1";

            return await GetAsync(uri);
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