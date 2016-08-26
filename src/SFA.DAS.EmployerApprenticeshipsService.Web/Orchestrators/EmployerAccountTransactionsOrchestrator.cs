using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
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

        public async Task<TransactionLineItemViewResult> GetAccounTransactionLineItem(int accountId, string lineItemId, string externalUserId)
        {
            var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountQuery
            {
                AccountId = accountId,
                ExternalUserId = externalUserId
            });
            if (employerAccountResult.Account == null)
            {
                return new TransactionLineItemViewResult();
            }

            var data = await _mediator.SendAsync(new GetEmployerAccountTransactionsQuery
            {
                AccountId = accountId
            });
            var latestLineItem = data.Data.Data.FirstOrDefault();
            decimal currentBalance;
            DateTime currentBalanceCalcultedOn;

            if (latestLineItem != null)
            {
                currentBalance = latestLineItem.Balance;
                currentBalanceCalcultedOn = new DateTime(latestLineItem.Year, latestLineItem.Month, 1);
            }
            else
            {
                currentBalance = 0;
                currentBalanceCalcultedOn = DateTime.Today;
            }

            var selectedLineItem = data.Data.Data.FirstOrDefault(line => line.Id == lineItemId);
            return new TransactionLineItemViewResult
            {
                Account = employerAccountResult.Account,
                Model = new TransactionLineItemViewModel
                {
                    CurrentBalance = currentBalance,
                    CurrentBalanceCalcultedOn = currentBalanceCalcultedOn,
                    LineItem = selectedLineItem
                }
            };
        }

        public async Task<TransactionViewResult> GetAccountTransactions(int accountId, string externalUserId)
        {
            var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountQuery
            {
                AccountId = accountId,
                ExternalUserId = externalUserId
            });
            if (employerAccountResult.Account == null)
            {
                return new TransactionViewResult();
            }

            var data = await _mediator.SendAsync(new GetEmployerAccountTransactionsQuery {AccountId = accountId});
            var latestLineItem = data.Data.Data.FirstOrDefault();
            decimal currentBalance;
            DateTime currentBalanceCalcultedOn;

            if (latestLineItem != null)
            {
                currentBalance = latestLineItem.Balance;
                currentBalanceCalcultedOn = new DateTime(latestLineItem.Year, latestLineItem.Month, 1);
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

    public class TransactionLineItemViewResult
    {
        public Account Account   { get; set; }
        public TransactionLineItemViewModel Model { get; set; }
    }

    public class TransactionLineItemViewModel
    {
        public AggregationLine LineItem { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime CurrentBalanceCalcultedOn { get; set; }
    }

    public class TransactionViewResult
    {
        public Account Account { get; set; }
        public TransactionViewModel Model { get; set; }
    }
}