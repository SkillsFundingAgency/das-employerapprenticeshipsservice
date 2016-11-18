using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.FindEmployerAccountLevyDeclarationTransactions;
using SFA.DAS.EAS.Application.Queries.FindEmployerAccountPaymentTransactions;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class EmployerAccountTransactionsOrchestrator
    {
        private readonly IMediator _mediator;

        public EmployerAccountTransactionsOrchestrator(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
        }

        public async Task<TransactionLineItemViewResult<LevyDeclarationTransactionLine>> FindAccountLevyDeclarationTransactions(
            string hashedId, DateTime fromDate, DateTime toDate, string externalUserId)
        {
            var data = await _mediator.SendAsync(new FindEmployerAccountLevyDeclarationTransactionsQuery
            {
                HashedAccountId = hashedId,
                FromDate = fromDate,
                ToDate = toDate,
                ExternalUserId = externalUserId
            });
           
            return new TransactionLineItemViewResult<LevyDeclarationTransactionLine>
            {
                Model = new TransactionLineViewModel<LevyDeclarationTransactionLine>
                {
                    Amount = data.Total,
                    SubTransactions = data.Transactions
                }
            };
        }

        public async Task<TransactionLineItemViewResult<PaymentTransactionLine>> FindAccountPaymentTransactions(
            string hashedId, DateTime fromDate, DateTime toDate, string externalUserId)
        {
            var data = await _mediator.SendAsync(new FindEmployerAccountPaymentTransactionsQuery
            {
                HashedAccountId = hashedId,
                FromDate = fromDate,
                ToDate = toDate,
                ExternalUserId = externalUserId
            });

            return new TransactionLineItemViewResult<PaymentTransactionLine>
            {
                Model = new TransactionLineViewModel<PaymentTransactionLine>
                {
                    Amount = data.Total,
                    SubTransactions = data.Transactions
                }
            };
        }

        public async Task<TransactionViewResult> GetAccountTransactions(string hashedId, string externalUserId)
        {
            var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountHashedQuery
            {
                HashedId = hashedId,
                UserId = externalUserId
            });
            if (employerAccountResult.Account == null)
            {
                return new TransactionViewResult();
            }

            var data = await _mediator.SendAsync(new GetEmployerAccountTransactionsQuery {AccountId = employerAccountResult.Account.Id,ExternalUserId = externalUserId,HashedId = hashedId});
            var latestLineItem = data.Data.TransactionLines.FirstOrDefault();
            decimal currentBalance;
            DateTime currentBalanceCalcultedOn;

            if (latestLineItem != null)
            {
                currentBalance = latestLineItem.Balance;
                currentBalanceCalcultedOn = latestLineItem.TransactionDate;
            }
            else
            {
                currentBalance = 0;
                currentBalanceCalcultedOn = DateTime.Today;
            }

            return new TransactionViewResult
            {
                Account = employerAccountResult.Account,
                Model = new TransactionViewModel
                {
                    CurrentBalance = currentBalance,
                    CurrentBalanceCalcultedOn = currentBalanceCalcultedOn,
                    Data = this.SortDataForViewModel(data.Data)
                }
            };
        }
        
        private AggregationData SortDataForViewModel(AggregationData data)
        {
            return data;
        }
    }
}