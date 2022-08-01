using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public class AccountTransactionsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILog _logger;
        private readonly IEmployerFinanceApiService _employerFinanceApiService;

        public AccountTransactionsOrchestrator(IMediator mediator, IMapper mapper, ILog logger, IEmployerFinanceApiService employerFinanceApiService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
            _employerFinanceApiService = employerFinanceApiService;
        }

        public async Task<OrchestratorResponse<SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel>> GetAccountTransactions(string hashedAccountId, int year, int month, UrlHelper urlHelper)
        {
            var data = await _employerFinanceApiService.GetTransactions(hashedAccountId, year, month);

            var transactionLists = data.Select(x => _mapper.Map<SFA.DAS.EAS.Account.Api.Types.TransactionViewModel>(x)).ToList();
            var transactionsViewModel = new TransactionsViewModel()
            {
                HasPreviousTransactions = data.HasPreviousTransactions, 
                Year = data.Year,
                Month = data.Month,
                PreviousMonthUri = data.PreviousMonthUri
            };
            transactionsViewModel.AddRange(transactionLists);

            var response = new OrchestratorResponse<SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel>
            {
                Data = transactionsViewModel
            };

            return response;
        }

        public async Task<OrchestratorResponse<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel>>> GetAccountTransactionSummary(string hashedAccountId)
        {   

            var data = await _employerFinanceApiService.GetTransactionSummary(hashedAccountId);            
            
            var transactionSummaryViewModel = data.Select(x => _mapper.Map<SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel>(x)).ToList();
            
            var response = new OrchestratorResponse<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel>>
            {
                Data = new AccountResourceList<SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel>(transactionSummaryViewModel)
            };
            
            return response;
        }        
    }
}