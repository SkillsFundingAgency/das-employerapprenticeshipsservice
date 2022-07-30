using System;
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
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
//using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public class AccountsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly IHashingService _hashingService;
        private readonly IEmployerAccountsApiService _employerAccountsApiService;
        private readonly IEmployerFinanceApiService _employerFinanceApiService;

        public AccountsOrchestrator(
            IMediator mediator, 
            ILog logger, 
            IMapper mapper, 
            IHashingService hashingService,
            IEmployerAccountsApiService employerAccountsApiService,
            IEmployerFinanceApiService employerFinanceApiService)
        {    
            _mediator = mediator;
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

            var accountBalanceTask = GetAccountBalance(accountResult.AccountId);
            var transferBalanceTask = GetTransferAllowanceForAccount(accountResult.AccountId);

            await Task.WhenAll(accountBalanceTask, transferBalanceTask).ConfigureAwait(false);

            accountResult.Balance = accountBalanceTask.Result?.Balance ?? 0;
            accountResult.RemainingTransferAllowance = transferBalanceTask.Result.RemainingTransferAllowance ?? 0;
            accountResult.StartingTransferAllowance = transferBalanceTask.Result.StartingTransferAllowance ?? 0;
            accountResult.IsAllowedPaymentOnService = IsAccountAllowedPaymentOnService(
                accountResult.AccountAgreementType,
                (ApprenticeshipEmployerType)Enum.Parse(typeof(ApprenticeshipEmployerType), accountResult.ApprenticeshipEmployerType), 
                accountBalanceTask.Result.LevyOverride);

            return new OrchestratorResponse<AccountDetailViewModel> { Data = accountResult };
        }

        public async Task<OrchestratorResponse<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>>> GetLevy(string hashedAccountId)
        {
            _logger.Info($"Requesting levy declaration for account {hashedAccountId}");

            //var levyDeclarations = await _mediator.SendAsync(new GetLevyDeclarationRequest { HashedAccountId = hashedAccountId });
            var levyDeclarations = await _employerFinanceApiService.GetLevyDeclarations(hashedAccountId);            
            if (levyDeclarations == null)
            {
                return new OrchestratorResponse<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>> { Data = null };
            }
/*
            List<LevyDeclarationViewModel> lists = new List<LevyDeclarationViewModel>();

            foreach(var test in levyDeclarations)
            {
                lists.Add(new LevyDeclarationViewModel
                {
                    CreatedDate = test.CreatedDate,
                    DateCeased = test.DateCeased,
                    EndOfYearAdjustment = test.EndOfYearAdjustment,
                    EndOfYearAdjustmentAmount = test.EndOfYearAdjustmentAmount,
                    EnglishFraction = test.EnglishFraction,
                    HashedAccountId = test.HashedAccountId,
                    HmrcSubmissionId = test.HmrcSubmissionId,
                    InactiveFrom = test.InactiveFrom,
                    InactiveTo = test.InactiveTo,
                    Id = test.Id,
                    LevyAllowanceForYear = test.LevyAllowanceForYear,
                    LevyDeclaredInMonth = test.LevyDeclaredInMonth,
                    LevyDueYtd = test.LevyDueYtd,
                    PayeSchemeReference = test.PayeSchemeReference,
                    PayrollMonth = test.PayrollMonth,
                    PayrollYear = test.PayrollYear,
                    SubmissionDate = test.SubmissionDate,
                    SubmissionId = test.SubmissionId,
                    TopUp = test.TopUp,
                    TopUpPercentage = test.TopUpPercentage,
                    TotalAmount = test.TotalAmount
                });
            };

            lists.ForEach(x => x.HashedAccountId = hashedAccountId);
            var levyViewModels = levyDeclarations.Select(x => _mapper.Map<SFA.DAS.EAS.Finance.Api.Types.LevyDeclarationViewModel, SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>(x)).ToList();*/
            
            var levyViewModels = levyDeclarations.Select(x => _mapper.Map<SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>(x)).ToList();
            levyViewModels.ForEach(x => x.HashedAccountId = hashedAccountId);

            return new OrchestratorResponse<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>>
            {
                Data = new AccountResourceList<SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>(levyViewModels),
                Status = HttpStatusCode.OK
            };
        }

        public async Task<OrchestratorResponse<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>>> GetLevy(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            _logger.Info($"Requesting levy declaration for account {hashedAccountId}, year {payrollYear} and month {payrollMonth}");

            //var levyDeclarations = await _mediator.SendAsync(new GetLevyDeclarationsByAccountAndPeriodRequest { HashedAccountId = hashedAccountId, PayrollYear = payrollYear, PayrollMonth = payrollMonth });
            var levyDeclarations = await _employerFinanceApiService.GetLevyForPeriod(hashedAccountId, payrollYear, payrollMonth);
            if (levyDeclarations == null)
            {
                return new OrchestratorResponse<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>> { Data = null };
            }

            var levyViewModels = levyDeclarations.Select(x => _mapper.Map<SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>(x)).ToList();
            levyViewModels.ForEach(x => x.HashedAccountId = hashedAccountId);

            return new OrchestratorResponse<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>>
            {
                Data = new AccountResourceList<SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>(levyViewModels),
                Status = HttpStatusCode.OK
            };
        }


       /* TODO: code which return account.api.types and throws an exception with Unit Test AutoMapper
        public async Task<OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>> GetLevy(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            _logger.Info($"Requesting levy declaration for account {hashedAccountId}, year {payrollYear} and month {payrollMonth}");

            //var levyDeclarations = await _mediator.SendAsync(new GetLevyDeclarationsByAccountAndPeriodRequest { HashedAccountId = hashedAccountId, PayrollYear = payrollYear, PayrollMonth = payrollMonth });
            var levyDeclarations = await _employerFinanceApiService.GetLevyForPeriod(hashedAccountId, payrollYear, payrollMonth);
            if (levyDeclarations == null)
            {
                return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>> { Data = null };
            }

            var levyViewModels = levyDeclarations.Select(x => _mapper.Map<LevyDeclarationViewModel>(x)).ToList();
            levyViewModels.ForEach(x => x.HashedAccountId = hashedAccountId);

            return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>
            {
                Data = new AccountResourceList<LevyDeclarationViewModel>(levyViewModels),
                Status = HttpStatusCode.OK
            };
        }*/

        private async Task<AccountBalance> GetAccountBalance(long accountId)
        {
            var balanceResult = await _mediator.SendAsync(new GetAccountBalancesRequest
            {
                AccountIds = new List<long> { accountId }
            });

            var account = balanceResult?.Accounts?.SingleOrDefault();
            return account;
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