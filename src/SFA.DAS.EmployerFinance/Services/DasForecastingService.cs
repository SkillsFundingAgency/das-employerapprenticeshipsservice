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

        public async Task<AccountProjectionSummary> GetAccountProjectionSummary(long accountId)
        {
            AccountProjectionSummary accountProjectionSummary = null;

            try
            {
                _logger.Info($"Getting expiring funds for account ID: {accountId}");

                var accountProjectionSummaryResponse = await _apiClient.Get<AccountProjectionSummaryResponseItem>(new GetAccountProjectionSummaryRequest(accountId));

                if (accountProjectionSummaryResponse != null)
                {
                    accountProjectionSummary = MapFrom(accountProjectionSummaryResponse);
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Could not find expired funds for account ID: {accountId} when calling forecast API");
            }

            return accountProjectionSummary;
        }

        private static AccountProjectionSummary MapFrom(AccountProjectionSummaryResponseItem accountProjectionSummaryResponse)
        {
            return new AccountProjectionSummary
            {
                AccountId = accountProjectionSummaryResponse.AccountId,
                ProjectionGenerationDate = accountProjectionSummaryResponse.ProjectionGenerationDate,
                ProjectionCalulation = new ProjectedCalculation
                {
                    FundsIn = accountProjectionSummaryResponse.FundsIn,
                    FundsOut = accountProjectionSummaryResponse.FundsOut,
                    NumberOfMonths = accountProjectionSummaryResponse.NumberOfMonths
                },
                ExpiringAccountFunds = new ExpiringAccountFunds
                {
                    ExpiryAmounts = accountProjectionSummaryResponse.ExpiryAmounts.Select(x => new ExpiringFunds
                    {
                        Amount = x.Amount,
                        PayrollDate = x.PayrollDate
                    }).ToList()
                }
            };
        }
    }
}
