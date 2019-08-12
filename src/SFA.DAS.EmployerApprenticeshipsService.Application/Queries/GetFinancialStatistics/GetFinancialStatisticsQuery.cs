using MediatR;
using SFA.DAS.EAS.Application.Queries.GetFinancialStatistics;

namespace SFA.DAS.EAS.Application.Queries.GetStatistics
{
    public class GetFinancialStatisticsQuery : IAsyncRequest<GetFinancialStatisticsResponse>
    {
    }
}
