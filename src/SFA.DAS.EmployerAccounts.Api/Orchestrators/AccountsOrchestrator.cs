using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetAccountBalances;
using SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
{
    public class AccountsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public AccountsOrchestrator(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<PayeSchemeViewModel> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            _logger.Info($"Getting paye scheme {payeSchemeRef} for account {hashedAccountId}");

            var payeSchemeResult = await _mediator.SendAsync(new GetPayeSchemeByRefQuery { HashedAccountId = hashedAccountId, Ref = payeSchemeRef });
            if (payeSchemeResult.PayeScheme == null)
            {
                return null;
            }

            var viewModel = ConvertPayeSchemeToViewModel(hashedAccountId, payeSchemeResult);
            return viewModel;
        }

        public async Task<OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>> GetAllAccountsWithBalances(string toDate, int pageSize, int pageNumber)
        {
            _logger.Info("Getting all account balances.");

            toDate = toDate ?? DateTime.MaxValue.ToString("yyyyMMddHHmmss");

            var accountsResult = await _mediator.SendAsync(new GetPagedEmployerAccountsQuery
            {
                ToDate = toDate,
                PageSize = pageSize,
                PageNumber = pageNumber
            });

            var transactionResult = await _mediator.SendAsync(new GetAccountBalancesRequest
            {
                AccountIds = accountsResult.Accounts.Select(account => account.Id).ToList()
            });

            var data = new List<AccountWithBalanceViewModel>();

            var accountBalanceHash = BuildAccountBalanceHash(transactionResult.Accounts);

            accountsResult.Accounts.ForEach(account =>
            {
                var accountBalanceModel = new AccountWithBalanceViewModel
                {
                    AccountId = account.Id,
                    AccountName = account.Name,
                    AccountHashId = account.HashedId,
                    PublicAccountHashId = account.PublicHashedId,
                    IsLevyPayer = true
                };

                if (accountBalanceHash.TryGetValue(account.Id, out var accountBalance))
                {
                    accountBalanceModel.Balance = accountBalance.Balance;
                    accountBalanceModel.RemainingTransferAllowance = accountBalance.RemainingTransferAllowance;
                    accountBalanceModel.StartingTransferAllowance = accountBalance.StartingTransferAllowance;
                    accountBalanceModel.IsLevyPayer = accountBalance.IsLevyPayer == 1;
                }

                data.Add(accountBalanceModel);
            });

            return new OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>> { Data = new PagedApiResponseViewModel<AccountWithBalanceViewModel> { Data = data, Page = pageNumber, TotalPages = (accountsResult.AccountsCount / pageSize) + 1 } };
        }

        private PayeSchemeViewModel ConvertPayeSchemeToViewModel(string hashedAccountId, GetPayeSchemeByRefResponse payeSchemeResult)
        {
            var payeSchemeViewModel = new PayeSchemeViewModel
            {
                DasAccountId = hashedAccountId,
                Name = payeSchemeResult.PayeScheme.Name,
                Ref = payeSchemeResult.PayeScheme.Ref,
                AddedDate = payeSchemeResult.PayeScheme.AddedDate,
                RemovedDate = payeSchemeResult.PayeScheme.RemovedDate
            };

            return payeSchemeViewModel;
        }

        private Dictionary<long, AccountBalance> BuildAccountBalanceHash(List<AccountBalance> accountBalances)
        {
            var result = new Dictionary<long, AccountBalance>(accountBalances.Count);

            foreach (var balance in accountBalances)
            {
                result.Add(balance.AccountId, balance);
            }

            return result;
        }
    }
}