using MediatR;
using SFA.DAS.EAS.Finance.Api.Types;
using SFA.DAS.EmployerFinance.Queries.GetFinancialStatistics;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Api.Orchestrators
{
    public class StatisticsOrchestrator
    {
        private readonly IMediator _mediator;       

        public StatisticsOrchestrator(IMediator mediator)
        {
            _mediator = mediator;            
        }

        public virtual async Task<FinanceStatisticsViewModel> Get()
        {  
            var financialStatisticsQueryTask = _mediator.SendAsync(new GetFinancialStatisticsQuery());

            return new FinanceStatisticsViewModel
            {            
                TotalPayments = (await financialStatisticsQueryTask).TotalPayments
            };
        }
    }
}