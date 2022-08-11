using System;
using AutoMapper;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public class AccountsOrchestrator
    {   
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly IHashingService _hashingService;
        private readonly IEmployerAccountsApiService _employerAccountsApiService;
        private readonly IEmployerFinanceApiService _employerFinanceApiService;

        public AccountsOrchestrator(            
            ILog logger, 
            IMapper mapper, 
            IHashingService hashingService,
            IEmployerAccountsApiService employerAccountsApiService,
            IEmployerFinanceApiService employerFinanceApiService)
        {   
            _logger = logger;
            _mapper = mapper;
            _hashingService = hashingService;
            _employerAccountsApiService = employerAccountsApiService;
            _employerFinanceApiService = employerFinanceApiService;
        }

        public async Task<OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>> GetAllAccountsWithBalances(string toDate, int pageSize, int pageNumber)
        {
            _logger.Info("Getting all account balances.");
            
            var accountsResult = await _employerAccountsApiService.GetAccounts(toDate, pageSize, pageNumber);            
           
            var transactionResult = await _employerFinanceApiService.GetAccountBalances(accountsResult.Data.Select(account => account.AccountHashId).ToList());
            var accountBalanceHash = BuildAccountBalanceHash(transactionResult);

            accountsResult.Data.ForEach(account =>
            {
                if (accountBalanceHash.TryGetValue(account.AccountId, out var accountBalance))
                {
                    account.Balance = accountBalance.Balance;
                    account.RemainingTransferAllowance = accountBalance.RemainingTransferAllowance;
                    account.StartingTransferAllowance = accountBalance.StartingTransferAllowance;
                    account.IsAllowedPaymentOnService = IsAccountAllowedPaymentOnService(account.AccountAgreementType, account.ApprenticeshipEmployerType, accountBalance.LevyOverride);
                    account.IsLevyPayer = account.IsAllowedPaymentOnService;
                }
            });

            return new OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>
            {
                Data = accountsResult
            };
        }

        private bool IsAccountAllowedPaymentOnService(AccountAgreementType accountAgreementType, ApprenticeshipEmployerType apprenticeshipEmployerType, bool? levyOverride)
        {
            if (levyOverride.HasValue)
            {
                return levyOverride.Value;
            }

            if (accountAgreementType == AccountAgreementType.Unknown)
            {
                return false;
            }

            if (accountAgreementType == AccountAgreementType.NonLevyExpressionOfInterest || accountAgreementType == AccountAgreementType.Combined)
            {
                return true;
            }

            return apprenticeshipEmployerType == ApprenticeshipEmployerType.Levy;
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
           
            var accountBalanceTask = _employerFinanceApiService.GetAccountBalances(new List<string> { accountResult.HashedAccountId });
            
            var transferBalanceTask = _employerFinanceApiService.GetTransferAllowance(accountResult.HashedAccountId);

            await Task.WhenAll(accountBalanceTask, transferBalanceTask).ConfigureAwait(false);            

            accountResult.Balance = accountBalanceTask.Result?.FirstOrDefault().Balance ?? 0;
            accountResult.RemainingTransferAllowance = transferBalanceTask.Result.RemainingTransferAllowance ?? 0;
            accountResult.StartingTransferAllowance = transferBalanceTask.Result.StartingTransferAllowance ?? 0;
            accountResult.IsAllowedPaymentOnService = IsAccountAllowedPaymentOnService(
                accountResult.AccountAgreementType,
                (ApprenticeshipEmployerType)Enum.Parse(typeof(ApprenticeshipEmployerType), accountResult.ApprenticeshipEmployerType), 
                accountBalanceTask.Result.FirstOrDefault().LevyOverride);

            return new OrchestratorResponse<AccountDetailViewModel> { Data = accountResult };
        }

        public async Task<OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>> GetLevy(string hashedAccountId)
        {
            _logger.Info($"Requesting levy declaration for account {hashedAccountId} from employerFinanceApiService");

            var levyDeclarations = await _employerFinanceApiService.GetLevyDeclarations(hashedAccountId);            
            if (levyDeclarations == null)
            {
                return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>> { Data = null };
            }

            _logger.Info($"Received response for levy declaration for account {hashedAccountId} from employerFinanceApiService");

            return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>
            {
                Data = new AccountResourceList<LevyDeclarationViewModel>(levyDeclarations),
                Status = HttpStatusCode.OK
            };
        }

        public async Task<OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>> GetLevy(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            _logger.Info($"Requesting levy declaration for account {hashedAccountId}, year {payrollYear} and month {payrollMonth} from employerFinanceApiService");

            var levyDeclarations = await _employerFinanceApiService.GetLevyForPeriod(hashedAccountId, payrollYear, payrollMonth);
            if (levyDeclarations == null)
            {
                return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>> { Data = null };
            }

            var levyViewModels = levyDeclarations.Select(x => _mapper.Map<LevyDeclarationViewModel>(x)).ToList();
            levyViewModels.ForEach(x => x.HashedAccountId = hashedAccountId);

            _logger.Info($"Received response for levy declaration for account {hashedAccountId}, year {payrollYear} and month {payrollMonth} from employerFinanceApiService");

            return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>
            {
                Data = new AccountResourceList<LevyDeclarationViewModel>(levyViewModels),
                Status = HttpStatusCode.OK
            };
        }
    }
}