using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetFinancialStatistics;
using SFA.DAS.EmployerAccounts.Api.Client;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public class StatisticsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IEmployerAccountsApiClient _employerAccountsApiClient;

        public StatisticsOrchestrator(IMediator mediator, IEmployerAccountsApiClient employerAccountsApiClient)
        {
            _mediator = mediator;
            _employerAccountsApiClient = employerAccountsApiClient;
        }

        public virtual async Task<StatisticsViewModel> Get()
        {
            var getAccountStatisticsTask = _employerAccountsApiClient.GetStatistics();
            var financialStatisticsQueryTask = _mediator.SendAsync(new GetFinancialStatisticsQuery());

            var accountStatistics = await getAccountStatisticsTask;
            return new StatisticsViewModel
            {
                TotalAccounts = accountStatistics.TotalAccounts,
                TotalAgreements = accountStatistics.TotalAgreements,
                TotalLegalEntities = accountStatistics.TotalLegalEntities,
                TotalPayeSchemes = accountStatistics.TotalPayeSchemes,
                TotalPayments = (await financialStatisticsQueryTask).Statistics.TotalPayments
            };
        }
    }
}