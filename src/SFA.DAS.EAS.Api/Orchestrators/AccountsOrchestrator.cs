using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetBatchEmployerAccountTransactions;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccounts;

namespace SFA.DAS.EAS.Api.Orchestrators
{
    public class AccountsOrchestrator
    {
        private readonly IMediator _mediator;

        public AccountsOrchestrator(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
        }


        public async Task<OrchestratorResponse<List<AccountWithBalanceViewModel>>> GetAllAccountsWithBalances()
        {
            var accountsResult = await _mediator.SendAsync(new GetEmployerAccountsQuery());
            var transactionResult =
                await
                    _mediator.SendAsync(new GetBatchEmployerAccountTransactionsQuery()
                    {
                        AccountIds = accountsResult.Accounts.Select(account => account.Id).ToList()
                    });

            var data = new List<AccountWithBalanceViewModel>();

            accountsResult.Accounts.ForEach(account =>
            {
                var transactions =
                    transactionResult.Data.Find(aggregationData => aggregationData.AccountId == account.Id);
                var latestLineItem = transactions?.Data.FirstOrDefault();


                var currentBalance = latestLineItem?.Balance ?? 0;
                data.Add(new AccountWithBalanceViewModel() {AccountId = account.Id, AccountName = account.Name, AccountHashId = account.HashedId, Balance = currentBalance });
                

            });



            return new OrchestratorResponse<List<AccountWithBalanceViewModel>>() {Data = data};
        } 
    }

    public class AccountWithBalanceViewModel
    {
        public string AccountName { get; set; }

        public string AccountHashId { get; set; }

        public long AccountId { get; set; }

        public decimal Balance { get; set; }
    }
}