using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionSummary;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;
using TransactionItemType = SFA.DAS.EAS.Account.Api.Types.TransactionItemType;

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

            //TODO : map to the account.api types
            List<TransactionViewModel> lists = new List<TransactionViewModel>();

            foreach (var test in data)
            {
                lists.Add(new TransactionViewModel
                {
                    Amount = test.Amount,
                    Balance = test.Balance,
                    DateCreated = test.DateCreated,
                    Description = test.Description,
                    ResourceUri = test.ResourceUri,
                    //SubTransactions = test.SubTransactions
                    TransactionDate = test.TransactionDate,
                    TransactionType = (TransactionItemType)test.TransactionType
                });
            };

            var transactionsViewModel = new TransactionsViewModel()
            {
                HasPreviousTransactions = data.HasPreviousTransactions, 
                Year = data.Year,
                Month = data.Month,
                PreviousMonthUri = data.PreviousMonthUri
            };
            transactionsViewModel.AddRange(lists);

            var response = new OrchestratorResponse<SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel>
            {
                Data = transactionsViewModel
            };

            return response;
        }

        //public async Task<OrchestratorResponse<SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel>> GetAccountTransactions(string hashedAccountId, int year, int month, UrlHelper urlHelper)
        //{
        //    var data = await _employerFinanceApiService.GetTransactions(hashedAccountId, year, month);
        //    //TODO : map to the account.api types
        //    /*             
        //      var data =
        //        await
        //            _mediator.SendAsync(new GetEmployerAccountTransactionsQuery
        //            {
        //                Year = year,
        //                Month = month,
        //                HashedAccountId = hashedAccountId
        //            });

        //    var response = new OrchestratorResponse<TransactionsViewModel>
        //    {
        //        Data = new TransactionsViewModel
        //        {
        //            HasPreviousTransactions = data.AccountHasPreviousTransactions,
        //            Year = data.Year,
        //            Month = data.Month
        //        }
        //    };

        //    response.Data.AddRange(data.Data.TransactionLines.Select(x => ConvertToTransactionViewModel(hashedAccountId, x, urlHelper)));
        //    return response;       */


        //    var responseAccountApi = new SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel
        //    {
        //        //HasPreviousTransactions = data.HasPreviousTransactions,
        //        //Year = data.Year,
        //        //Month = data.Month,
        //        new TransactionViewModel
        //        {
        //            Amount = data.FirstOrDefault().Amount,
        //            Balance = data.FirstOrDefault().Balance,
        //            DateCreated = data.FirstOrDefault().DateCreated,
        //            Description = data.FirstOrDefault().Description,
        //            ResourceUri = data.FirstOrDefault().ResourceUri,
        //            //SubTransactions =  data.FirstOrDefault().SubTransactions,
        //            TransactionDate = data.FirstOrDefault().TransactionDate,
        //            TransactionType = (TransactionItemType)data.FirstOrDefault().TransactionType
        //        }

        //    };

        //    var response = new OrchestratorResponse<SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel>
        //    {
        //        Data = responseAccountApi
        //    };


        //   /*  var response = _mapper.Map<SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel>(data);

        //    var response = new OrchestratorResponse<SFA.DAS.EAS.Account.Api.Types.TransactionsViewModel>
        //    {
        //        Data = afterMapping
        //    };*/

        //    return response;
        //}

        public async Task<OrchestratorResponse<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel>>> GetAccountTransactionSummary(string hashedAccountId)
        {   

            var data = await _employerFinanceApiService.GetTransactionSummary(hashedAccountId);            
            
            //TODO : why automapper not working ????
           // var transactionSummaryViewModel = data.Select(x => _mapper.Map<SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel>(x)).ToList();


            List<TransactionSummaryViewModel> lists = new List<TransactionSummaryViewModel>();

            foreach (var test in data)
            {
                lists.Add(new TransactionSummaryViewModel
                {
                   Amount = test.Amount,
                   Href = test.Href,
                   Month = test.Month,
                   Year = test.Year
                });
            };


            var response = new OrchestratorResponse<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel>>
            {
                Data = new AccountResourceList<SFA.DAS.EAS.Account.Api.Types.TransactionSummaryViewModel>(lists)
            };
            
            return response;
        }
        
    }
}