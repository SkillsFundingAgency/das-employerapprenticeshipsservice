using MediatR;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.AccountTransactions.GetAccountProviderPayments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class DasLevyService : IDasLevyService
    {
        private readonly IMediator _mediator;

        public DasLevyService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ICollection<T>> GetAccountProviderPaymentsByDateRange<T>(
            long accountId, long ukprn, DateTime fromDate, DateTime toDate) where T : TransactionLine
        {
            var result = await _mediator.SendAsync(new GetAccountProviderPaymentsByDateRangeQuery
            {
                AccountId = accountId,
                UkPrn = ukprn,
                FromDate = fromDate,
                ToDate = toDate
            });

            return result?.Transactions?.OfType<T>().ToList() ?? new List<T>();
        }
    }
}
