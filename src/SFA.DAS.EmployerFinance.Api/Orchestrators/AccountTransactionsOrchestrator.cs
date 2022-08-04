using MediatR;
using SFA.DAS.EAS.Finance.Api.Types;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.GetAccountTransactionSummary;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccountTransactions;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace SFA.DAS.EmployerFinance.Api.Orchestrators
{
    public class AccountTransactionsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public AccountTransactionsOrchestrator(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
      
        public async Task<OrchestratorResponse<TransactionsViewModel>> GetAccountTransactions(string hashedAccountId, int year, int month, UrlHelper urlHelper)
        {
            var data =
                await
                    _mediator.SendAsync(new GetEmployerAccountTransactionsQuery
                    {
                        Year = year,
                        Month = month,
                        HashedAccountId = hashedAccountId
                    });

            var response = new OrchestratorResponse<TransactionsViewModel>
            {
                Data = new TransactionsViewModel
                {
                    HasPreviousTransactions = data.AccountHasPreviousTransactions,
                    Year = data.Year,
                    Month = data.Month
                }
            };

            response.Data.AddRange(data.Data.TransactionLines.Select(x => ConvertToTransactionViewModel(hashedAccountId, x, urlHelper)));
            return response;
        }       

        public async Task<List<TransactionSummary>> GetAccountTransactionSummary(string hashedAccountId)
        {            
            var response = await _mediator.SendAsync(new GetAccountTransactionSummaryRequest { HashedAccountId = hashedAccountId });
            if (response.Data == null)
            {
                return null;
            }          

            return response.Data;
        }
     

        private TransactionViewModel ConvertToTransactionViewModel(string hashedAccountId, Models.Transaction.TransactionLine transactionLine, UrlHelper urlHelper)
        {
            var viewModel = new TransactionViewModel
            {
                Amount = transactionLine.Amount,
                Balance = transactionLine.Balance,
                Description = transactionLine.Description,
                TransactionType = (EAS.Finance.Api.Types.TransactionItemType)transactionLine.TransactionType,
                DateCreated = transactionLine.DateCreated,
                SubTransactions = transactionLine.SubTransactions?.Select(x => ConvertToTransactionViewModel(hashedAccountId, x, urlHelper)).ToList(),
                TransactionDate = transactionLine.TransactionDate
            };

            if (transactionLine.TransactionType == SFA.DAS.EmployerFinance.Models.Transaction.TransactionItemType.Declaration)
            {
                viewModel.ResourceUri = urlHelper.Route("GetLevyForPeriod", new { hashedAccountId, payrollYear = transactionLine.PayrollYear, payrollMonth = transactionLine.PayrollMonth });
            }

            return viewModel;
        }
    }
}