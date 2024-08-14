using System.Collections.Concurrent;
using System.Net.Http;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Support.Infrastructure.Services;

public sealed class AccountRepository : IAccountRepository
{
    private readonly IEncodingService _encodingService;
    private readonly IEmployerAccountsApiService _apiService;
    private readonly IAccountApiClient _accountApiClient;
    private readonly IDatetimeService _datetimeService;
    private readonly ILogger<AccountRepository> _logger;
    private readonly IPayeSchemeObfuscator _payeSchemeObfuscator;
    private readonly IPayRefHashingService _hashingService;

    public AccountRepository(IAccountApiClient accountApiClient,
        IPayeSchemeObfuscator payeSchemeObfuscator,
        IDatetimeService datetimeService,
        ILogger<AccountRepository> logger,
        IPayRefHashingService hashingService,
        IEncodingService encodingService,
        IEmployerAccountsApiService apiService)
    {
        _apiService = apiService;
        _accountApiClient = accountApiClient;
        _payeSchemeObfuscator = payeSchemeObfuscator;
        _datetimeService = datetimeService;
        _logger = logger;
        _hashingService = hashingService;
        _encodingService = encodingService;
    }

    public async Task<Core.Models.Account> Get(string hashedAccountId, AccountFieldsSelection selection)
    {
        try
        {
            var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
            var response = await _apiService.GetAccount(accountId);

            return await GetAdditionalFields(response, selection);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Account with id {Id} not found", hashedAccountId);
            return null;
        }
    }

    public async Task<IEnumerable<Core.Models.Account>> FindAllDetails(int pageSize, int pageNumber)
    {
        var results = new List<Core.Models.Account>();

        try
        {
            var accountPageModel = await _apiService.GetAccounts(null, pageSize, pageNumber);
            if (accountPageModel?.Data?.Count > 0)
            {
                var accountsDetail = await GetAccountSearchDetails(accountPageModel.Data);
                results.AddRange(accountsDetail);
            }
        }
        catch (HttpRequestException requestException)
        {
            _logger.LogWarning("The Account API Http request threw an exception while fetching Page {PageNumber} - Exception :\r\n{RequestException}", pageNumber, requestException);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"A general exception has been thrown while requesting employer account details");
        }

        _logger.LogDebug("Account Details data Page ({PageNumber} Size {pagesize}) : {Results}", pageNumber, pageSize, JsonConvert.SerializeObject(results));

        return results;
    }

    public async Task<int> TotalAccountRecords(int pagesize)
    {
        try
        {
            var accountFirstPageModel = await _apiService.GetAccounts(null, 1, pagesize);
            if (accountFirstPageModel == null)
            {
                return 0;
            }

            return accountFirstPageModel.TotalPages * pagesize;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Exception while loading all account details");
            throw;
        }
    }



    private async Task<IEnumerable<Core.Models.Account>> GetAccountSearchDetails(IEnumerable<AccountWithBalanceViewModel> accounts)
    {
        var accountsWithDetails = new ConcurrentBag<Core.Models.Account>();

        await Parallel.ForEachAsync(accounts, async (acc, cancellationToken) =>
        {
            try
            {
                _logger.LogInformation("Getting account {AccountId}", acc.AccountId);
                var account = await _apiService.GetAccount(acc.AccountId, cancellationToken);

                _logger.LogInformation("GetAdditionalFields for account ID {AccountId}", account.AccountId);
                var accountWithDetails = await GetAdditionalFields(account, AccountFieldsSelection.PayeSchemes);
                accountsWithDetails.Add(accountWithDetails);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception while retrieving details for account ID {AccountId}", acc.AccountId);
            }
        });

        return accountsWithDetails;
    }

    private async Task<Core.Models.Account> GetAdditionalFields(AccountDetailViewModel response, AccountFieldsSelection selection)
    {
        var result = MapToAccount(response);

        switch (selection)
        {
            case AccountFieldsSelection.Organisations:
                _logger.LogInformation("Getting Organisations for the account {AccountId}", result.AccountId);
                var legalEntities = await GetLegalEntities(response.LegalEntities ?? new ResourceList(new List<ResourceViewModel>()));
                result.LegalEntities = legalEntities;
                break;
            case AccountFieldsSelection.TeamMembers:
                _logger.LogInformation("Getting TeamMembers for the account {AccountId}", result.AccountId);
                var teamMembers = await GetAccountTeamMembers(result.AccountId);
                result.TeamMembers = teamMembers;
                break;
            case AccountFieldsSelection.PayeSchemes:
                _logger.LogInformation("Getting PayeSchemes for the account {AccountId}", result.AccountId);
                result.PayeSchemes = await MapToDomainPayeSchemeAsync(response);
                return result;
            case AccountFieldsSelection.Finance:
                _logger.LogInformation("Getting PayeSchemes and Transactions for the {AccountId}", result.AccountId);
                result.PayeSchemes = await MapToDomainPayeSchemeAsync(response);
                result.Transactions = await GetAccountTransactions(response.HashedAccountId);
                return result;
        }

        return result;
    }

    private async Task<List<TransactionViewModel>> GetAccountTransactions(string accountId)
    {
        var endDate = DateTime.Now.Date;
        var financialYearIterator = _datetimeService.GetBeginningFinancialYear(new DateTime(2017, 4, 1));
        var response = new List<TransactionViewModel>();

        while (financialYearIterator <= endDate)
        {
            try
            {
                var transactions = await _accountApiClient.GetTransactions(accountId, financialYearIterator.Year,
                    financialYearIterator.Month);
                response.AddRange(transactions);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception occured in Account API type of {TransactionsViewModelName} for period {FinancialYearIteratorYear}.{FinancialYearIteratorMonth} id {AccountId}", nameof(TransactionsViewModel), financialYearIterator.Year, financialYearIterator.Month, accountId);
            }

            financialYearIterator = financialYearIterator.AddMonths(1);
        }

        return GetFilteredTransactions(response);
    }

    private static List<TransactionViewModel> GetFilteredTransactions(IEnumerable<TransactionViewModel> response)
    {
        return response.Where(x => x.Description != null && x.Amount != 0).OrderByDescending(x => x.DateCreated).ToList();
    }

    private async Task<IEnumerable<PayeSchemeModel>> GetPayeSchemes(AccountDetailViewModel response)
    {
        var payes = new List<PayeSchemeModel>();

        var payesBatches = response.PayeSchemes
            .Select((item, inx) => new { item, inx })
            .GroupBy(x => x.inx / 50)
            .Select(g => g.Select(x => x.item));

        foreach (var payeBatch in payesBatches)
        {
            var payeTasks = payeBatch.Select(payeScheme =>
            {
                var obscured = _payeSchemeObfuscator.ObscurePayeScheme(payeScheme.Id).Replace("/", "%252f");
                var paye = payeScheme.Id.Replace("/", "%252f");
                _logger.LogDebug("IAccountApiClient.GetResource<PayeSchemeViewModel>(\"{Obscured}\");", payeScheme.Href.Replace(paye, obscured));

                return _apiService.GetResource<PayeSchemeModel>(payeScheme.Href).ContinueWith(payeTask =>
                {
                    if (!payeTask.IsFaulted)
                    {
                        return payeTask.Result;
                    }

                    _logger.LogError(payeTask.Exception, "Exception occured in Account API type of {PayeSchemeViewModelName} at {PayeSchemeHref} id {PayeSchemeId}", nameof(PayeSchemeModel), payeScheme.Href, payeScheme.Id);
                    return new PayeSchemeModel();
                });
            });

            payes.AddRange(await Task.WhenAll(payeTasks));
        }

        return payes.Select(payeSchemeViewModel =>
            {
                if (!IsValidPayeScheme(payeSchemeViewModel))
                {
                    return null;
                }

                var item = new PayeSchemeModel
                {
                    Ref = payeSchemeViewModel.Ref,
                    DasAccountId = payeSchemeViewModel.DasAccountId,
                    AddedDate = payeSchemeViewModel.AddedDate,
                    RemovedDate = payeSchemeViewModel.RemovedDate,
                    Name = payeSchemeViewModel.Name
                };

                return item;
            }).Where(x => x != null)
            .OrderBy(x => x.Ref);
    }

    private static bool IsValidPayeScheme(PayeSchemeModel result)
    {
        return result.AddedDate <= DateTime.UtcNow &&
               (result.RemovedDate == null || result.RemovedDate > DateTime.UtcNow);
    }

    private async Task<ICollection<TeamMemberViewModel>> GetAccountTeamMembers(long accountId)
    {
        try
        {
            var teamMembers = await _apiService.GetAccountUsers(accountId);

            return teamMembers;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Account Team Members for Account: ({AccountId}) not found", accountId);
            return new List<TeamMemberViewModel>();
        }
    }

    private async Task<List<LegalEntityViewModel>> GetLegalEntities(ResourceList responseLegalEntities)
    {
        var legalEntitiesList = new List<LegalEntityViewModel>();

        foreach (var legalEntity in responseLegalEntities)
        {
            try
            {
                var legalResponse = await _apiService.GetResource<LegalEntityViewModel>(legalEntity.Href);

                if (legalResponse.AgreementStatus == EmployerAgreementStatus.Signed ||
                    legalResponse.AgreementStatus == EmployerAgreementStatus.Pending ||
                    legalResponse.AgreementStatus == EmployerAgreementStatus.Superseded)
                    legalEntitiesList.Add(legalResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured calling Account API for type of {LegalEntityViewModelName} at {LegalEntityHref} id {LegalEntityId}", nameof(LegalEntityViewModel), legalEntity.Href, legalEntity.Id);
            }
        }

        return legalEntitiesList;
    }

    private static Core.Models.Account MapToAccount(AccountDetailViewModel accountDetailViewModel)
    {
        return new Core.Models.Account
        {
            AccountId = accountDetailViewModel.AccountId,
            DasAccountName = accountDetailViewModel.DasAccountName,
            HashedAccountId = accountDetailViewModel.HashedAccountId,
            PublicHashedAccountId = accountDetailViewModel.PublicHashedAccountId,
            DateRegistered = accountDetailViewModel.DateRegistered,
            OwnerEmail = accountDetailViewModel.OwnerEmail,
            ApprenticeshipEmployerType = accountDetailViewModel.ApprenticeshipEmployerType
        };
    }

    private async Task<IEnumerable<PayeSchemeModel>> MapToDomainPayeSchemeAsync(AccountDetailViewModel response)
    {
        var mainPayeSchemes = await GetPayeSchemes(response);

        return mainPayeSchemes?.Select(payeScheme => new PayeSchemeModel
        {
            Ref = payeScheme.Ref,
            DasAccountId = payeScheme.DasAccountId,
            AddedDate = payeScheme.AddedDate,
            RemovedDate = payeScheme.RemovedDate,
            Name = payeScheme.Name,
            HashedPayeRef = _hashingService.HashValue(payeScheme.Ref),
            ObscuredPayeRef = _payeSchemeObfuscator.ObscurePayeScheme(payeScheme.Ref)
        }).ToList();
    }
}