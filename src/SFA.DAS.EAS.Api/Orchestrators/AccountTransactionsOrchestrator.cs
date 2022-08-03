﻿using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
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

        public async Task<OrchestratorResponse<TransactionsViewModel>> GetAccountTransactions(string hashedAccountId, int year, int month, UrlHelper urlHelper)
        {
            //TODO : add logs
            var data = await _employerFinanceApiService.GetTransactions(hashedAccountId, year, month);

            var transactionLists = data.Select(x => _mapper.Map<TransactionViewModel>(x)).ToList();
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
            //TODO : add logs
            var data = await _employerFinanceApiService.GetTransactionSummary(hashedAccountId);            
            
            var transactionSummaryViewModel = data.Select(x => _mapper.Map<TransactionSummaryViewModel>(x)).ToList();
            
            var response = new OrchestratorResponse<AccountResourceList<TransactionSummaryViewModel>>
            {
                Data = new AccountResourceList<TransactionSummaryViewModel>(transactionSummaryViewModel)
            };
            
            return response;
        }        
    }
}