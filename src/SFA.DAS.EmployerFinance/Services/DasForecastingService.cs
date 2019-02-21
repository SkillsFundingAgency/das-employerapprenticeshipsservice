using System.Threading.Tasks;
using SFA.DAS.ActiveDirectory;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Http;

namespace SFA.DAS.EmployerFinance.Services
{
    public class DasForecastingService : IDasForecastingService
    {
        private readonly IHttpClientWrapper _httpClient;
        private readonly IAzureAdAuthenticationService _azureAdAuthService;
        private readonly ForecastingApiClientConfiguration _apiClientConfiguration;
        private readonly ILog _logger;

        public DasForecastingService(IHttpClientWrapper httpClient,
            IAzureAdAuthenticationService azureAdAuthService,
            ForecastingApiClientConfiguration apiClientConfiguration,
            ILog logger)
        {
            _httpClient = httpClient;
            _azureAdAuthService = azureAdAuthService;
            _apiClientConfiguration = apiClientConfiguration;
            _logger = logger;

            _httpClient.BaseUrl = _apiClientConfiguration.ApiBaseUrl;
            _httpClient.AuthScheme = "Bearer";
        }

        public async Task<ExpiringAccountFunds> GetExpiringAccountFunds(long accountId)
        {
            _logger.Info($"Getting expiring funds for account ID: {accountId}");

            var expiredFundsUrl = $"/api/accounts/{accountId}/AccountProjection/expiring-funds";

            var accessToken = await _azureAdAuthService.GetAuthenticationResult(
                _apiClientConfiguration.ClientId,
                _apiClientConfiguration.ClientSecret,
                _apiClientConfiguration.IdentifierUri,
                _apiClientConfiguration.Tenant);

            var accountExpiredFunds =
               await _httpClient.Get<ExpiringAccountFunds>(accessToken, expiredFundsUrl);

            return accountExpiredFunds;
        }
    }
}
