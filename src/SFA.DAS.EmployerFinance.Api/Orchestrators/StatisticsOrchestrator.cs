using MediatR;
using SFA.DAS.EmployerFinance.Api.Types;
using SFA.DAS.EmployerFinance.Queries.GetTotalPayments;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Api.Orchestrators
{
    public class StatisticsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public StatisticsOrchestrator(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public virtual async Task<TotalPaymentsModel> Get()
        {
            _logger.Info($"Requesting Finance statistics");

            var financialStatisticsQueryTask = _mediator.SendAsync(new GetTotalPaymentsQuery());

            return new TotalPaymentsModel
            {            
                TotalPayments = (await financialStatisticsQueryTask).TotalPayments
            };
        }
    }
}