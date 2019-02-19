using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Models.ExpiringFunds;
using SFA.DAS.EmployerFinance.Queries.GetExpiringAccountFunds;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountFinanceOverview
{
    public class GetAccountFinanceOverviewQueryHandler : IAsyncRequestHandler<GetAccountFinanceOverviewQuery, GetAccountFinanceOverviewResponse>
    {
        private readonly IForecastingService _forecastingService;
        private readonly IDasLevyService _levyService;
        private readonly ILog _logger;

        public GetAccountFinanceOverviewQueryHandler(
            IForecastingService forecastingService,
            IDasLevyService levyService, 
            ILog logger)
        {
            _forecastingService = forecastingService;
            _levyService = levyService;
            _logger = logger;
        }

        public async Task<GetAccountFinanceOverviewResponse> Handle(GetAccountFinanceOverviewQuery query)
        {
            if (!query.AccountId.HasValue)
            {
                _logger.Warn("Request made to get expiring funds with null account ID");
                return new GetAccountFinanceOverviewResponse();
            }

            var currentBalance = await GetAccountBalance(query.AccountId.Value);
            var earliestFundsToExpire = await GetExpiringFunds(query.AccountId.Value);

            if (earliestFundsToExpire == null)
            {
                return new GetAccountFinanceOverviewResponse
                {
                    AccountId = query.AccountId
                };
            }

            return new GetAccountFinanceOverviewResponse
            {
                AccountId = query.AccountId,
                CurrentFunds = currentBalance,
                ExpiringFundsExpiryDate = earliestFundsToExpire.PayrollDate,
                ExpiringFundsAmount = earliestFundsToExpire.Amount
            };
        }

        private async Task<ExpiringFunds> GetExpiringFunds(long accountId)
        {
            _logger.Info($"Getting expiring funds for account ID: {accountId}");

            var expiringFunds = await _forecastingService.GetExpiringAccountFunds(accountId);
            var earliestFundsToExpire = expiringFunds?.ExpiryAmounts?.OrderBy(a => a.PayrollDate).FirstOrDefault();
            
            return earliestFundsToExpire;
        }

        private async Task<decimal> GetAccountBalance(long accountId)
        {
            try
            {
                _logger.Info($"Getting current funds balance for account ID: {accountId}");
                
                return await _levyService.GetAccountBalance(accountId);
                
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Failed to get account's current balance for account ID: {accountId}");
                
                throw;
            }
        }
    }
}
