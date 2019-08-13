using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Application.Queries.GetFinancialStatistics
{
    public class GetFinancialStatisticsResponse
    {
        //todo: no need for this viewmodel level of indirection. its badly named also!
        public FinancialStatistics Statistics { get; set; }
    }
}
