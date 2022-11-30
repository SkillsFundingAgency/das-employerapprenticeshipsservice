using MediatR;
using SFA.DAS.EmployerFinance.Api.Types;
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
      
        public async Task<Transactions> GetAccountTransactions(string hashedAccountId, int year, int month, UrlHelper urlHelper)
        {
            _logger.Info($"Requesting account transactions for account {hashedAccountId}, year {year} and month {month}");

            var data = await _mediator.SendAsync(new GetEmployerAccountTransactionsQuery {
                        Year = year,
                        Month = month,
                        HashedAccountId = hashedAccountId
                    });

            var response = new Transactions
            {
                HasPreviousTransactions = data.AccountHasPreviousTransactions,
                Year = year,
                Month = month
            };
            response.AddRange(data.Data.TransactionLines.Select(x => ConvertToTransactionViewModel(hashedAccountId, x, urlHelper)));
            
            _logger.Info($"Received account transactions response for account {hashedAccountId}, year {year} and month {month}");
            return response;
        }

        public async Task<List<TransactionSummary>> GetAccountTransactionSummary(string hashedAccountId)
        {
            _logger.Info($"Requesting account transaction summary for account {hashedAccountId}");

            var response = await _mediator.SendAsync(new GetAccountTransactionSummaryRequest { HashedAccountId = hashedAccountId });
            if (response.Data == null)
            {
                return null;
            }
            _logger.Info($"Received account transaction summary response for account {hashedAccountId}");
            return response.Data;
        }     

        private Transaction ConvertToTransactionViewModel(string hashedAccountId, Models.Transaction.TransactionLine transactionLine, UrlHelper urlHelper)
        {
            var viewModel = new Transaction
            {
                Amount = transactionLine.Amount,
                Balance = transactionLine.Balance,
                Description = transactionLine.Description,
                TransactionType = (TransactionItemType)transactionLine.TransactionType,
                DateCreated = transactionLine.DateCreated,
                SubTransactions = transactionLine.SubTransactions?.Select(x => ConvertToTransactionViewModel(hashedAccountId, x, urlHelper)).ToList(),
                TransactionDate = transactionLine.TransactionDate
            };

            if (transactionLine.TransactionType == Models.Transaction.TransactionItemType.Declaration)
            {
                viewModel.ResourceUri = urlHelper.Route("GetLevyForPeriod", new { hashedAccountId, payrollYear = transactionLine.PayrollYear, payrollMonth = transactionLine.PayrollMonth });
            }

            return viewModel;
        }
    }
}