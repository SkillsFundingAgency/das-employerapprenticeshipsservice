using System.Threading.Tasks;
using SFA.DAS.ActiveDirectory;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.NLog.Logger;
using SFA.DAS.EmployerFinance.Models.ProjectedCalculations;
using System;
using SFA.DAS.EmployerFinance.Http;

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

            return await CallAuthService<ExpiringAccountFunds>(
                accountId,
                (ex) =>
                {
                    if (ex is ResourceNotFoundException)
                    {
                        _logger.Error(ex, $"Could not find expired funds for account ID: {accountId} when calling forecast API");
                    }
                }
                );
        }

        public async Task<ProjectedCalculation> GetProjectedCalculations(long accountId)
        {
            _logger.Info($"Getting projected calculations for account ID: {accountId}");

            return await CallAuthService<ProjectedCalculation>(
                accountId,
                (ex) =>
                {
                    if (ex is ResourceNotFoundException)
                    {
                        _logger.Error(ex, $"Could not find projected calculations for account ID: {accountId} when calling forecast API");
                    }
                }
                );
        }

        private async Task<T> CallAuthService<T>(long accountId, Action<Exception> OnError = null)
        {
            var accessToken = await _azureAdAuthService.GetAuthenticationResult(
                _apiClientConfiguration.ClientId,
                _apiClientConfiguration.ClientSecret,
                _apiClientConfiguration.IdentifierUri,
                _apiClientConfiguration.Tenant);

            try
            {
                return await _httpClient.Get<T>(accessToken, GetUrl(typeof(T), accountId));
            }
            catch (ResourceNotFoundException ex)
            {
                OnError?.Invoke(ex);
                _logger.Error(ex, $"ResourceNotFoundException returned from forecast API for account ID: {accountId}");                
                return default(T);
            }
            catch (HttpException ex)
            {
                OnError?.Invoke(ex);

                switch (ex.StatusCode)
                {
                    case 400:
                        _logger.Error(ex, $"Bad request sent to forecast API for account ID: {accountId}");
                        break;
                    case 408:
                        _logger.Error(ex, $"Request sent to forecast API for account ID: {accountId} timed out");
                        break;
                    case 429:
                        _logger.Error(ex, $"To many requests sent to forecast API for account ID: {accountId}");
                        break;
                    case 500:
                        _logger.Error(ex, $"Forecast API reported internal Server error for account ID: {accountId}");
                        break;
                    case 503:
                        _logger.Error(ex, "Forecast API is unavailable");
                        break;

                    default: throw;
                }

                return default(T);
            }
        }

        private string GetUrl(Type type, long accountId)
        {
            string url = $"/api/accounts/{accountId}/AccountProjection/";
            if (type.IsAssignableFrom(typeof(ProjectedCalculation)))
            {
                url = url + "projected-summary";
            };
            if (type.IsAssignableFrom(typeof(ExpiringAccountFunds)))
            {
                url = url + "expiring-funds";
            };

            return url;
        }
    }
}
