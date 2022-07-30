using MediatR;
using SFA.DAS.EmployerFinance.Data;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetFinancialStatistics
{
    public class GetFinancialStatisticsQueryHandler
        : IAsyncRequestHandler<GetFinancialStatisticsQuery, GetFinancialStatisticsResponse>
    {
        private readonly EmployerFinanceDbContext _financeDb;

        public GetFinancialStatisticsQueryHandler(EmployerFinanceDbContext financeDb)
        {
            _financeDb = financeDb;
        }

        public async Task<GetFinancialStatisticsResponse> Handle(GetFinancialStatisticsQuery message)
        {
            return new GetFinancialStatisticsResponse
            {
                TotalPayments = await _financeDb.Payments.CountAsync()
            };
        }
    }
}
