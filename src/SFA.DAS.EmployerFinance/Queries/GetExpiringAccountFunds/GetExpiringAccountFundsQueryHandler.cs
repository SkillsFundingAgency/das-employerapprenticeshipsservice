using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Queries.GetExpiringAccountFunds
{
    public class GetExpiringAccountFundsQueryHandler : IAsyncRequestHandler<GetExpiringAccountFundsQuery, GetExpiringAccountFundsResponse>
    {
        private readonly IForecastingService _forecastingService;
        private readonly ILog _logger;

        public GetExpiringAccountFundsQueryHandler(IForecastingService forecastingService, ILog logger)
        {
            _forecastingService = forecastingService;
            _logger = logger;
        }

        public async Task<GetExpiringAccountFundsResponse> Handle(GetExpiringAccountFundsQuery query)
        {
            _logger.Info($"Getting expiring funds for account ID: {query.AccountId}");

            var expiringFunds = await _forecastingService.GetExpiringAccountFunds(query.AccountId);
           
            var earliestFundsToExpire = expiringFunds?.ExpiryAmounts?.OrderBy(a => a.PayrollDate).FirstOrDefault();

            if (earliestFundsToExpire == null)
            {
                return new GetExpiringAccountFundsResponse
                {
                    AccountId = query.AccountId
                };
            }

            return new GetExpiringAccountFundsResponse
            {
                AccountId = query.AccountId,
                ExpiryDate = earliestFundsToExpire.PayrollDate,
                Amount = earliestFundsToExpire.Amount
            };
        }
    }
}
