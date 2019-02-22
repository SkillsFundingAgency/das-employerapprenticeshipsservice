using System;
using System.Net;
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

            try
            {
                var accountExpiredFunds =
                    await _httpClient.Get<ExpiringAccountFunds>(accessToken, expiredFundsUrl);

                return accountExpiredFunds;
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.Error(ex, $"Could not find expired funds for account ID: {accountId} when calling forecast API");
                
                return null;
            }
            catch (HttpException ex)
            {
                switch (ex.StatusCode)
                {
                    case 400 : _logger.Error(ex, $"Bad request sent to forecast API for account ID: {accountId}");
                        break;
                    case 408 : _logger.Error(ex, $"Request sent to forecast API for account ID: {accountId} timed out");
                        break;
                    case 429 : _logger.Error(ex, $"To many requests sent to forecast API for account ID: {accountId}");
                        break;
                    case 500 : _logger.Error(ex, $"Forecast API reported internal Server error for account ID: {accountId}");
                        break;
                    case 503 : _logger.Error(ex, "Forecast API is unavailable");
                        break;

                    default: throw;
                }

                return null;
            }
        }
    }
}
