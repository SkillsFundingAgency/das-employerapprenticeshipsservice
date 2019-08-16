using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetFinancialStatistics;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public class StatisticsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IEmployerAccountsApiService _employerAccountsApiService;

        public StatisticsOrchestrator(IMediator mediator, IEmployerAccountsApiService EmployerAccountsApiService)
        {
            _mediator = mediator;
            _employerAccountsApiService = EmployerAccountsApiService;
        }

        public virtual async Task<StatisticsViewModel> Get()
        {
            var getAccountStatisticsTask = _employerAccountsApiService.GetStatistics();
            var financialStatisticsQueryTask = _mediator.SendAsync(new GetFinancialStatisticsQuery());

            var accountStatistics = await getAccountStatisticsTask;
            return new StatisticsViewModel
            {
                TotalAccounts = accountStatistics.TotalAccounts,
                TotalAgreements = accountStatistics.TotalAgreements,
                TotalLegalEntities = accountStatistics.TotalLegalEntities,
                TotalPayeSchemes = accountStatistics.TotalPayeSchemes,
                TotalPayments = (await financialStatisticsQueryTask).TotalPayments
            };
        }
    }
}