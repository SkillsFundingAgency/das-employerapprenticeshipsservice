using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public class EmployerAccountsApiClient : IEmployerAccountsApiClient
    {
        private readonly IEmployerAccountsApiClientConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        public EmployerAccountsApiClient(IEmployerAccountsApiClientConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new SecureHttpClient(configuration);
        }

        public Task HealthCheck()
        {
            var baseUrl = GetBaseUrl();
            var url = $"{baseUrl}/api/healthcheck";

            return _httpClient.GetAsync(url);
        }

        private string GetBaseUrl()
        {
            return _configuration.ApiBaseUrl.Trim('/');
        }
    }
}