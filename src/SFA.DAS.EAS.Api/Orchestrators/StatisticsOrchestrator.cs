using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetTheRdsRequiredStatistics;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public interface IStatisticsOrchestrator
    {
        Task<OrchestratorResponse<RdsRequiredStatisticsViewModel>> GetTheRdsRequiredStatistics();
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

        public async Task<OrchestratorResponse<RdsRequiredStatisticsViewModel>> GetTheRdsRequiredStatistics()
        {
            _logger.Debug($"Fetching the statistics required for RDS at {DateTime.UtcNow}");

            var response = await _mediator.SendAsync(new GetTheRdsRequiredStatisticsRequest());

            return new OrchestratorResponse<RdsRequiredStatisticsViewModel>
            {
                Data = response.Statistics.IsEmpty() ? null : response.Statistics
            };
        }
    }
}