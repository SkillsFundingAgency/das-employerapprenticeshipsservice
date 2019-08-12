using System.Data.Entity;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Application.Queries.GetFinancialStatistics
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
            // no need for Future when only need to make 1 trip
            return new GetFinancialStatisticsResponse
            {
                Statistics = new FinancialStatisticsViewModel
                {
                    TotalPayments = await _financeDb.Payments.CountAsync()
                }
            };
        }
    }
}