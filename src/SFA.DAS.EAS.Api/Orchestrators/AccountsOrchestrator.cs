using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Account.Api.Orchestrators;

public class AccountsOrchestrator
{   
    private readonly ILogger<AccountsOrchestrator> _logger;
    private readonly IMapper _mapper;
    private readonly IEncodingService _encodingService;
    private readonly IEmployerAccountsApiService _employerAccountsApiService;
    private readonly IEmployerFinanceApiService _employerFinanceApiService;

    public AccountsOrchestrator(
        ILogger<AccountsOrchestrator> logger, 
        IMapper mapper,
        IEncodingService encodingService,
        IEmployerAccountsApiService employerAccountsApiService,
        IEmployerFinanceApiService employerFinanceApiService)
    {   
        _logger = logger;
        _mapper = mapper;
        _encodingService = encodingService;
        _employerAccountsApiService = employerAccountsApiService;
        _employerFinanceApiService = employerFinanceApiService;
    }

    public async Task<OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>> GetAllAccountsWithBalances(string toDate, int pageSize, int pageNumber)
    {
        _logger.LogInformation("Getting all account balances.");
            
        var accountsResult = await _employerAccountsApiService.GetAccounts(toDate, pageSize, pageNumber);

        _logger.LogInformation("calling finance api service to GetAccountBalances");
        var transactionResult = await _employerFinanceApiService.GetAccountBalances(accountsResult.Data.Select(account => account.AccountHashId).ToList());
        var accountBalanceHash = BuildAccountBalanceHash(transactionResult);
        _logger.LogInformation("received response from finance api service to GetAccountBalances {TransactionResultCount} ", transactionResult.Count);

        accountsResult.Data.ForEach(account =>
        {
            if (!accountBalanceHash.TryGetValue(account.AccountId, out var accountBalance))
            {
                return;
            }
                
            account.Balance = accountBalance.Balance;
            account.RemainingTransferAllowance = accountBalance.RemainingTransferAllowance;
            account.StartingTransferAllowance = accountBalance.StartingTransferAllowance;
            account.IsAllowedPaymentOnService = IsAccountAllowedPaymentOnService(account.AccountAgreementType, account.ApprenticeshipEmployerType, accountBalance.LevyOverride);
            account.IsLevyPayer = account.IsAllowedPaymentOnService;
        });

        return new OrchestratorResponse<PagedApiResponseViewModel<AccountWithBalanceViewModel>>
        {
            Data = accountsResult
        };
    }

    private static bool IsAccountAllowedPaymentOnService(AccountAgreementType accountAgreementType, ApprenticeshipEmployerType apprenticeshipEmployerType, bool? levyOverride)
    {
        if (levyOverride.HasValue)
        {
            return levyOverride.Value;
        }

        switch (accountAgreementType)
        {
            case AccountAgreementType.Unknown:
                return false;
            case AccountAgreementType.NonLevyExpressionOfInterest:
            case AccountAgreementType.Combined:
                return true;
            default:
                return apprenticeshipEmployerType == ApprenticeshipEmployerType.Levy;
        }
    }

    private static Dictionary<long, AccountBalance> BuildAccountBalanceHash(List<AccountBalance> accountBalances)
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
        var hashedAccountId = _encodingService.Encode(accountId, EncodingType.AccountId);

        if (string.IsNullOrWhiteSpace(hashedAccountId))
        {
            return new OrchestratorResponse<AccountDetailViewModel> { Data = null };
        }

        var response = await GetAccount(hashedAccountId);
        return response;
    }

    public async Task<OrchestratorResponse<AccountDetailViewModel>> GetAccount(string hashedAccountId)
    {
        _logger.LogInformation("Getting account {HashedAccountId}", hashedAccountId);
        
        var accountResult = await _employerAccountsApiService.GetAccount(hashedAccountId);

        if (accountResult.AccountId == 0)
        {
            return new OrchestratorResponse<AccountDetailViewModel> { Data = null };
        }
           
        var accountBalanceTask = _employerFinanceApiService.GetAccountBalances(new List<string> { accountResult.HashedAccountId });
            
        var transferBalanceTask = _employerFinanceApiService.GetTransferAllowance(accountResult.HashedAccountId);

        await Task.WhenAll(accountBalanceTask, transferBalanceTask).ConfigureAwait(false);            

        accountResult.Balance = accountBalanceTask.Result?.FirstOrDefault()?.Balance ?? 0;
        accountResult.RemainingTransferAllowance = transferBalanceTask.Result.RemainingTransferAllowance ?? 0;
        accountResult.StartingTransferAllowance = transferBalanceTask.Result.StartingTransferAllowance ?? 0;
        
        accountResult.IsAllowedPaymentOnService = IsAccountAllowedPaymentOnService(
            accountResult.AccountAgreementType,
            (ApprenticeshipEmployerType)Enum.Parse(typeof(ApprenticeshipEmployerType), accountResult.ApprenticeshipEmployerType), 
            accountBalanceTask.Result.FirstOrDefault()?.LevyOverride
            );

        return new OrchestratorResponse<AccountDetailViewModel> { Data = accountResult };
    }

    public async Task<OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>> GetLevy(string hashedAccountId)
    {
        _logger.LogInformation("Requesting levy declaration for account {HashedAccountId} from employerFinanceApiService", hashedAccountId);

        var levyDeclarations = await _employerFinanceApiService.GetLevyDeclarations(hashedAccountId);            
        if (levyDeclarations == null)
        {
            return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>> { Data = null };
        }

        _logger.LogInformation("Received response for levy declaration for account {HashedAccountId} from employerFinanceApiService", hashedAccountId);

        return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>
        {
            Data = new AccountResourceList<LevyDeclarationViewModel>(levyDeclarations),
            Status = HttpStatusCode.OK
        };
    }

    public async Task<OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>> GetLevy(string hashedAccountId, string payrollYear, short payrollMonth)
    {
        _logger.LogInformation("Requesting levy declaration for account {HashedAccountId}, year {PayrollYear} and month {PayrollMonth} from employerFinanceApiService", hashedAccountId, payrollYear, payrollMonth);

        var levyDeclarations = await _employerFinanceApiService.GetLevyForPeriod(hashedAccountId, payrollYear, payrollMonth);
        if (levyDeclarations == null)
        {
            return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>> { Data = null };
        }

        var levyViewModels = levyDeclarations.Select(x => _mapper.Map<LevyDeclarationViewModel>(x)).ToList();
        levyViewModels.ForEach(x => x.HashedAccountId = hashedAccountId);

        _logger.LogInformation("Received response for levy declaration for account {HashedAccountId}, year {PayrollYear} and month {PayrollMonth} from employerFinanceApiService", hashedAccountId, payrollYear, payrollMonth);

        return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>
        {
            Data = new AccountResourceList<LevyDeclarationViewModel>(levyViewModels),
            Status = HttpStatusCode.OK
        };
    }
}