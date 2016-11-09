using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactionDetail;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Levy;
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

        public async Task<TransactionLineItemViewResult> GetAccounTransactionLineItem(string hashedId, long lineItemId, string externalUserId)
        {
            var data = await _mediator.SendAsync(new GetEmployerAccountTransactionDetailQuery
            {
                HashedId = hashedId,
                ExternalUserId = externalUserId,
                Id = lineItemId
            });
           
            return new TransactionLineItemViewResult
            {
                Model = new TransactionLineItemViewModel
                {
                    TotalAmount = data.Total,
                    LineItem = data.TransactionDetail
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

    public class TransactionLineItemViewResult
    {
        public Account Account   { get; set; }
        public TransactionLineItemViewModel Model { get; set; }
    }

    public class TransactionLineItemViewModel
    {
        public List<TransactionDetailSummary> LineItem { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class TransactionViewResult
    {
        public Account Account { get; set; }
        public TransactionViewModel Model { get; set; }
    }
}