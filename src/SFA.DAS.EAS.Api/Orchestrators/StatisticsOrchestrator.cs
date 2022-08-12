using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public class StatisticsOrchestrator
    {   
        private readonly IEmployerAccountsApiService _employerAccountsApiService;
        private readonly IEmployerFinanceApiService _employerFinanceApiService;

        public StatisticsOrchestrator(IEmployerAccountsApiService employerAccountsApiService, IEmployerFinanceApiService employerFinanceApiService)
        {   
            _employerAccountsApiService = employerAccountsApiService;
            _employerFinanceApiService = employerFinanceApiService;
        }

        public virtual async Task<StatisticsViewModel> Get()
        {
            var getAccountStatisticsTask = _employerAccountsApiService.GetStatistics();
            var financialStatisticsQueryTask = _employerFinanceApiService.GetStatistics();

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