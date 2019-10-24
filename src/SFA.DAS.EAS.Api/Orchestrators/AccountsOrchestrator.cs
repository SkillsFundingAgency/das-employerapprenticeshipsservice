using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Application.Queries.GetLevyDeclaration;
using SFA.DAS.EAS.Application.Queries.GetLevyDeclarationsByAccountAndPeriod;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public class AccountsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly IHashingService _hashingService;
        private readonly IEmployerAccountsApiService _employerAccountsApiService;

        public AccountsOrchestrator(
            IMediator mediator, 
            ILog logger, 
            IMapper mapper, 
            IHashingService hashingService,
            IEmployerAccountsApiService employerAccountsApiService)
        {    
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _hashingService = hashingService;
            _employerAccountsApiService = employerAccountsApiService;
        }

        public async Task<OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>> GetAllAccountsWithBalances(string toDate, int pageSize, int pageNumber)
        {
            _logger.Info("Getting all account balances.");
            
            var accountsResult = await _employerAccountsApiService.GetAccounts(toDate, pageSize, pageNumber);

            var transactionResult = await _mediator.SendAsync(new GetAccountBalancesRequest
            {
                AccountIds = accountsResult.Data.Select(account => account.AccountId).ToList()
            });
       
            var accountBalanceHash = BuildAccountBalanceHash(transactionResult.Accounts);

            accountsResult.Data.ForEach(account =>
            {
                if (accountBalanceHash.TryGetValue(account.AccountId, out var accountBalance))
                {
                    account.Balance = accountBalance.Balance;
                    account.RemainingTransferAllowance = accountBalance.RemainingTransferAllowance;
                    account.StartingTransferAllowance = accountBalance.StartingTransferAllowance;
                }
            });

            return new OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>
            {
                Data = accountsResult
            };
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

        public async Task<OrchestratorResponse<AccountDetailViewModel>> GetAccount(long accountId)
        {
            var hashedAccountId = _hashingService.HashValue(accountId);

            if (string.IsNullOrWhiteSpace(hashedAccountId))
            {
                return new OrchestratorResponse<AccountDetailViewModel> { Data = null };
            }

            var response = await GetAccount(hashedAccountId);
            return response;
        }

        public async Task<OrchestratorResponse<AccountDetailViewModel>> GetAccount(string hashedAccountId)
        {
            _logger.Info($"Getting account {hashedAccountId}");
        
            var accountResult = await _employerAccountsApiService.GetAccount(hashedAccountId);

            if (accountResult.AccountId == 0)
            {
                return new OrchestratorResponse<AccountDetailViewModel> { Data = null };
            }

            var accountBalanceTask = GetBalanceForAccount(accountResult.AccountId);
            var transferBalanceTask = GetTransferAllowanceForAccount(accountResult.AccountId);

            await Task.WhenAll(accountBalanceTask, transferBalanceTask).ConfigureAwait(false);

            accountResult.Balance = accountBalanceTask.Result;
            accountResult.RemainingTransferAllowance = transferBalanceTask.Result.RemainingTransferAllowance ?? 0;
            accountResult.StartingTransferAllowance = transferBalanceTask.Result.StartingTransferAllowance ?? 0;

            return new OrchestratorResponse<AccountDetailViewModel> { Data = accountResult };
        }

        public async Task<OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>> GetLevy(string hashedAccountId)
        {
            _logger.Info($"Requesting levy declaration for account {hashedAccountId}");

            var levyDeclarations = await _mediator.SendAsync(new GetLevyDeclarationRequest { HashedAccountId = hashedAccountId });
            if (levyDeclarations.Declarations == null)
            {
                return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>> { Data = null };
            }

            var levyViewModels = levyDeclarations.Declarations.Select(x => _mapper.Map<LevyDeclarationViewModel>(x)).ToList();
            levyViewModels.ForEach(x => x.HashedAccountId = hashedAccountId);

            return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>
            {
                Data = new AccountResourceList<LevyDeclarationViewModel>(levyViewModels),
                Status = HttpStatusCode.OK
            };
        }

        public async Task<OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>> GetLevy(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            _logger.Info($"Requesting levy declaration for account {hashedAccountId}, year {payrollYear} and month {payrollMonth}");

            var levyDeclarations = await _mediator.SendAsync(new GetLevyDeclarationsByAccountAndPeriodRequest { HashedAccountId = hashedAccountId, PayrollYear = payrollYear, PayrollMonth = payrollMonth });
            if (levyDeclarations.Declarations == null)
            {
                return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>> { Data = null };
            }

            var levyViewModels = levyDeclarations.Declarations.Select(x => _mapper.Map<LevyDeclarationViewModel>(x)).ToList();
            levyViewModels.ForEach(x => x.HashedAccountId = hashedAccountId);

            return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>
            {
                Data = new AccountResourceList<LevyDeclarationViewModel>(levyViewModels),
                Status = HttpStatusCode.OK
            };
        }

        private async Task<decimal> GetBalanceForAccount(long accountId)
        {
            var balanceResult = await _mediator.SendAsync(new GetAccountBalancesRequest
            {
                AccountIds = new List<long> { accountId }
            });

            var account = balanceResult?.Accounts?.SingleOrDefault();
            return account?.Balance ?? 0;
        }

        private async Task<TransferAllowance> GetTransferAllowanceForAccount(long accountId)
        {
            var transferAllowanceResult = await _mediator.SendAsync(new GetTransferAllowanceQuery
            {
                AccountId = accountId
            });

            return transferAllowanceResult.TransferAllowance;
        }
    }
}