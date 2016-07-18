using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
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

        public async Task<TransactionViewResult>  GetAccountTransactions(int accountId)
        {
            var employerAccountResult = await _mediator.SendAsync(new GetEmployerAccountQuery { Id = accountId });
            if (employerAccountResult == null)
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

        private decimal CurrentBalanceForViewModel(AggregationData data)
        {
            throw new NotImplementedException();
        }

        private AggregationData SortDataForViewModel(AggregationData data)
        {
            return data;
        }
    }

    public class TransactionViewResult
    {
        public Account Account { get; set; }
        public TransactionViewModel Model { get; set; }
    }
}