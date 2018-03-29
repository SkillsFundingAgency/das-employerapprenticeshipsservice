using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetStatistics;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public interface IStatisticsOrchestrator
    {
        Task<OrchestratorResponse<StatisticsViewModel>> GetStatistics();
    }

    public class StatisticsOrchestrator : IStatisticsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public StatisticsOrchestrator(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<OrchestratorResponse<StatisticsViewModel>> GetStatistics()
        {
            _logger.Debug($"Fetching the statistics required for RDS at {DateTime.UtcNow}");

            var response = await _mediator.SendAsync(new GetStatisticsRequest());

            return new OrchestratorResponse<StatisticsViewModel>
            {
                Data = response.Statistics.IsEmpty() ? null : response.Statistics
            };
        }
    }
}