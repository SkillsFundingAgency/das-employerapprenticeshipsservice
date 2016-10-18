using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using SFA.DAS.EAS.Api.Models;
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


        public async Task<OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>> GetAllAccountsWithBalances(string fromDate, int pageSize, int pageNumber)
        {
            var accountsResult = await _mediator.SendAsync(new GetEmployerAccountsQuery());
            var transactionResult = await _mediator.SendAsync(new GetBatchEmployerAccountTransactionsQuery()
            {
                AccountIds = accountsResult.Accounts.Select(account => account.Id).ToList()
            });

            var data = new List<AccountWithBalanceViewModel>();

            accountsResult.Accounts.ForEach(account =>
            {
                var transactions = transactionResult.Data.Find(aggregationData => aggregationData.AccountId == account.Id);
                var latestLineItem = transactions?.Data.FirstOrDefault();
                var currentBalance = latestLineItem?.Balance ?? 0;
                data.Add(new AccountWithBalanceViewModel() { AccountId = account.Id, AccountName = account.Name, AccountHashId = account.HashedId, Balance = currentBalance });
            });

            return new OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>() {Data = new PagedApiResponseViewModel<AccountWithBalanceViewModel>() {Data = data, Page = 0, TotalPages = 0} };
        }

    }
}