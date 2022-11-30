using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Projections;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Projections;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.EmployerFinance.Models.ProjectedCalculations;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Services
{
    public class DasForecastingService : IDasForecastingService
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ILog _logger;

        public DasForecastingService(IOuterApiClient apiClient, ILog logger)
        {
            _outerApiClient = apiClient;
            _logger = logger;
        }

        public async Task<AccountProjectionSummary> GetAccountProjectionSummary(long accountId)
        {
            AccountProjectionSummary accountProjectionSummary = null;

            try
            {
                _logger.Info($"Getting forecasting projection summary for account ID: {accountId}");

                var accountProjectionSummaryResponse = await _outerApiClient.Get<GetAccountProjectionSummaryResponse>(new GetAccountProjectionSummaryRequest(accountId));

                if (accountProjectionSummaryResponse != null)
                {
                    accountProjectionSummary = MapFrom(accountProjectionSummaryResponse);
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Could not find forecasting projection summary for account ID: {accountId} when calling forecast API");
            }

            return accountProjectionSummary;
        }

        private static AccountProjectionSummary MapFrom(GetAccountProjectionSummaryResponse accountProjectionSummaryResponse)
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
