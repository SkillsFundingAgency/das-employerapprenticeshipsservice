using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Models;
using SFA.DAS.EntityFramework;

namespace SFA.DAS.EmployerFinance.Queries.GetStatistics
{
    public class GetStatisticsQueryHandler : IAsyncRequestHandler<GetStatisticsQuery, GetStatisticsResponse>
    {
        private readonly EmployerFinanceDbContextReadOnly _financeDb;

        public GetStatisticsQueryHandler(EmployerFinanceDbContextReadOnly financeDb)
        {
            _financeDb = financeDb;
        }

        public async Task<GetStatisticsResponse> Handle(GetStatisticsQuery message)
        {
            // TODO When decoupling
            //var accountsQuery = _accountDb.Value.Accounts.FutureCount();
            //var payeSchemesQuery = _accountDb.Value.Payees.FutureCount();
            var paymentsQuery = _financeDb.Payments.FutureCount();

            var statistics = new StatisticsViewModel
            {
                //TotalAccounts = await accountsQuery.ValueAsync(),
                //TotalPayeSchemes = await payeSchemesQuery.ValueAsync(),
                TotalPayments = await paymentsQuery.ValueAsync()
            };

            return new GetStatisticsResponse
            {
                Statistics = statistics
            };
        }
    }
}