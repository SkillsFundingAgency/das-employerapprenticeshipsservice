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
            return new GetFinancialStatisticsResponse
            {
                Statistics = new FinancialStatistics
                {
                    TotalPayments = await _financeDb.Payments.CountAsync()
                }
            };
        }
    }
}