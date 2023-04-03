using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.NLog.Logger;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public class AccountTransactionsOrchestrator
    {        
        private readonly ILogger<AccountTransactionsOrchestrator> _logger;
        private readonly IEmployerFinanceApiService _employerFinanceApiService;

        public AccountTransactionsOrchestrator(ILogger<AccountTransactionsOrchestrator> logger, 
            IEmployerFinanceApiService employerFinanceApiService
        )
        {
            _logger = logger;
            _employerFinanceApiService = employerFinanceApiService;
        }

        public async Task<OrchestratorResponse<TransactionsViewModel>> GetAccountTransactions(string hashedAccountId, int year, int month)
        {
            _logger.LogInformation($"Requesting GetAccountTransactions for account {hashedAccountId} from employerFinanceApiService");

            var data = await _employerFinanceApiService.GetTransactions(hashedAccountId, year, month);

            var transactionLists = data.Select(x => x).ToList();
            var transactionsViewModel = new TransactionsViewModel()
            {
                HasPreviousTransactions = data.HasPreviousTransactions, 
                Year = data.Year,
                Month = data.Month,
                PreviousMonthUri = data.PreviousMonthUri
            };
            transactionsViewModel.AddRange(transactionLists);

            var response = new OrchestratorResponse<TransactionsViewModel>
            {
                Data = transactionsViewModel
            };

            return response;
        }

        public async Task<OrchestratorResponse<AccountResourceList<TransactionSummaryViewModel>>> GetAccountTransactionSummary(string hashedAccountId)
        {
            _logger.LogInformation($"Requesting AccountTransactionSummary for account {hashedAccountId} from employerFinanceApiService");

            var data = await _employerFinanceApiService.GetTransactionSummary(hashedAccountId);

            var response = new OrchestratorResponse<AccountResourceList<TransactionSummaryViewModel>>
            {
                Data = new AccountResourceList<TransactionSummaryViewModel>(data)
            };
            
            return response;
        }        
    }
}