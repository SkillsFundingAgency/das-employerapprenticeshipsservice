﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using MediatR;
using NLog;
using SFA.DAS.EAS.Api.Models;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetBatchEmployerAccountTransactions;
using SFA.DAS.EAS.Application.Queries.GetPagedEmployerAccounts;

namespace SFA.DAS.EAS.Api.Orchestrators
{
    public class AccountsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public AccountsOrchestrator(IMediator mediator, ILogger logger )
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
            _logger = logger;
        }


        public async Task<OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>> GetAllAccountsWithBalances(string toDate, int pageSize, int pageNumber)
        {
            toDate = toDate ?? DateTime.MaxValue.ToString("yyyyMMddHHmmss");

            var accountsResult = await _mediator.SendAsync(new GetPagedEmployerAccountsQuery() {ToDate = toDate, PageSize = pageSize, PageNumber = pageNumber});
            var transactionResult = await _mediator.SendAsync(new GetAccountBalancesRequest
            {
                AccountIds = accountsResult.Accounts.Select(account => account.Id).ToList()
            });

            var data = new List<AccountWithBalanceViewModel>();

            accountsResult.Accounts.ForEach(account =>
            {
                var accountBalance = transactionResult.Accounts.SingleOrDefault(c=>c.AccountId==account.Id);
                data.Add(new AccountWithBalanceViewModel() { AccountId = account.Id, AccountName = account.Name, AccountHashId = account.HashedId, Balance = accountBalance?.Balance ?? 0 });
            });

            return new OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>() {Data = new PagedApiResponseViewModel<AccountWithBalanceViewModel>() {Data = data, Page = pageNumber, TotalPages = (accountsResult.AccountsCount / pageSize) + 1} };
        }

    }
}