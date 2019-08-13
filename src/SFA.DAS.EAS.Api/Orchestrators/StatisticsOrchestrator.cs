using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetFinancialStatistics;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public class StatisticsOrchestrator
    {
        private readonly IMediator _mediator;

        public StatisticsOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public virtual async Task<FinancialStatisticsViewModel> Get()
        {
            var financialStatisticsTask = _mediator.SendAsync(new GetFinancialStatisticsQuery());
            var financialStatistics = await financialStatisticsTask;
            return financialStatistics.Statistics;
        }
    }
}