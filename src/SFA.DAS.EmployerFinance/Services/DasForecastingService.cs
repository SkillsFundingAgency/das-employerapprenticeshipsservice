using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.ActiveDirectory;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Http;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.EmployerFinance.Models.ProjectedCalculations;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Services
{
    public class DasForecastingService : IDasForecastingService
    {
        private readonly IApiClient _apiClient;
        private readonly IHttpClientWrapper _httpClient;
        private readonly IAzureAdAuthenticationService _azureAdAuthService;
        private readonly ForecastingApiClientConfiguration _apiClientConfiguration;
        private readonly ILog _logger;

        public DasForecastingService(IHttpClientWrapper httpClient,
            IAzureAdAuthenticationService azureAdAuthService,
            ForecastingApiClientConfiguration apiClientConfiguration, 
            IApiClient apiClient,
            ILog logger)
        {
            _httpClient = httpClient;
            _azureAdAuthService = azureAdAuthService;
            _apiClientConfiguration = apiClientConfiguration;
            _apiClient = apiClient;
            _logger = logger;

            _httpClient.BaseUrl = _apiClientConfiguration.ApiBaseUrl;
            _httpClient.AuthScheme = "Bearer";
        }

        public async Task<ExpiringAccountFunds> GetExpiringAccountFunds(long accountId)
        {
            ExpiringAccountFunds expiringAccountFunds = null;

            try
            {
                _logger.Info($"Getting expiring funds for account ID: {accountId}");

                var expiringAccountFundsResponse = await _apiClient.Get<ExpiringAccountFundsResponseItem>(new GetExpiringAccountFundsRequest(accountId));

                if (expiringAccountFundsResponse != null)
                {
                    expiringAccountFunds = MapFrom(expiringAccountFundsResponse);
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Could not find expired funds for account ID: {accountId} when calling forecast API");
            }

            return expiringAccountFunds;
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

        private static ExpiringAccountFunds MapFrom(ExpiringAccountFundsResponseItem expiringAccountFundsResponse)
        {
            return new ExpiringAccountFunds
            {
                AccountId = expiringAccountFundsResponse.AccountId,
                ProjectionGenerationDate = expiringAccountFundsResponse.ProjectionGenerationDate,
                ExpiryAmounts = expiringAccountFundsResponse.ExpiryAmounts.Select(x => new ExpiringFunds
                {
                    Amount = x.Amount,
                    PayrollDate = x.PayrollDate
                }).ToList()
            };
        }

        private async Task<T> CallAuthService<T>(long accountId, Action<Exception> OnError = null)
        {
            var accessToken = IsClientCredentialConfiguration(_apiClientConfiguration.ClientId, _apiClientConfiguration.ClientSecret, _apiClientConfiguration.Tenant)
                ? await _azureAdAuthService.GetAuthenticationResult(
                _apiClientConfiguration.ClientId,
                _apiClientConfiguration.ClientSecret,
                _apiClientConfiguration.IdentifierUri,
                _apiClientConfiguration.Tenant)
                : await GetManagedIdentityAuthenticationResult(_apiClientConfiguration.IdentifierUri);

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

        private async Task<string> GetManagedIdentityAuthenticationResult(string resource)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            return await azureServiceTokenProvider.GetAccessTokenAsync(resource);
        }

        private bool IsClientCredentialConfiguration(string clientId, string clientSecret, string tenant)
        {
            return !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret) && !string.IsNullOrEmpty(tenant);
        }
    }
}
