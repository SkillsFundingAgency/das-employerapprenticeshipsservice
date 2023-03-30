using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public sealed class AccountRepository : IAccountRepository
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly IDatetimeService _datetimeService;
        private readonly ILogger<AccountRepository> _logger;
        private readonly IPayeSchemeObfuscator _payeSchemeObfuscator;
        private readonly IHashingService _hashingService;

        public AccountRepository(IAccountApiClient accountApiClient,
            IPayeSchemeObfuscator payeSchemeObfuscator,
            IDatetimeService datetimeService,
            ILogger<AccountRepository> logger,
            IHashingService hashingService)
        {
            _accountApiClient = accountApiClient;
            _payeSchemeObfuscator = payeSchemeObfuscator;
            _datetimeService = datetimeService;
            _logger = logger;
            _hashingService = hashingService;
        }

        public async Task<Core.Models.Account> Get(string id, AccountFieldsSelection selection)
        {
            try
            {
                _logger.LogDebug(
                    $"{nameof(IAccountApiClient)}.{nameof(IAccountApiClient.GetResource)}<{nameof(AccountDetailViewModel)}>(\"/api/accounts/{id.ToUpper()}\");");

                var response = await _accountApiClient.GetResource<AccountDetailViewModel>($"/api/accounts/{id.ToUpper()}");

                return await GetAdditionalFields(response, selection);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Account with id {id} not found");
                return null;
            }
        }

        public async Task<IEnumerable<Core.Models.Account>> FindAllDetails(int pagesize, int pageNumber)
        {
            var results = new List<Core.Models.Account>();

            try
            {
                var accountPageModel = await _accountApiClient.GetPageOfAccounts(pageNumber, pagesize);
                if (accountPageModel?.Data?.Count > 0)
                {
                    var accountsDetail = await GetAccountSearchDetails(accountPageModel.Data);
                    results.AddRange(accountsDetail);
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogWarning($"The Account API Http request threw an exception while fetching Page {pageNumber} - Exception :\r\n{e}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"A general exception has been thrown while requesting employer account details");
            }
            _logger.LogDebug($"Account Details data Page ({pageNumber} Size {pagesize}) : {(JsonConvert.SerializeObject(results))}");
            return results;
        }

        public async Task<int> TotalAccountRecords(int pagesize)
        {
            try
            {
                var accountFirstPageModel = await _accountApiClient.GetPageOfAccounts(1, pagesize);
                if (accountFirstPageModel == null)
                {
                    return 0;
                }

                return (accountFirstPageModel.TotalPages * pagesize);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while loading all account details");

                throw;
            }
        }

        public async Task<decimal> GetAccountBalance(string id)
        {
            try
            {
                var response = await _accountApiClient.GetResource<AccountWithBalanceViewModel>($"/api/accounts/{id}");

                _logger.LogDebug(
                    $"{nameof(IAccountApiClient)}.{nameof(_accountApiClient.GetResource)}<{nameof(AccountWithBalanceViewModel)}>(\"/api/accounts/{id}\"); {response.Balance}");

                return response.Balance;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Account Balance with id {id} not found");
                return 0;
            }
        }

        private async Task<IEnumerable<Core.Models.Account>> GetAccountSearchDetails(IEnumerable<AccountWithBalanceViewModel> accounts)
        {
            var accountsWithDetails = new List<Core.Models.Account>();

            var accountDetailTasks = accounts.Select(account => _accountApiClient.GetAccount(account.AccountHashId));
            var accountDetails = await Task.WhenAll(accountDetailTasks);

            foreach (var account in accountDetails)
            {
                try
                {
                    _logger.LogInformation($"GetAdditionalFields for account ID {account.HashedAccountId}");
                    var accountWithDetails = await GetAdditionalFields(account, AccountFieldsSelection.PayeSchemes);
                    accountsWithDetails.Add(accountWithDetails);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Exception while retrieving details for account ID {account.HashedAccountId}");
                }
            }

            return accountsWithDetails;
        }

        private async Task<Core.Models.Account> GetAdditionalFields(AccountDetailViewModel response, AccountFieldsSelection selection)
        {
            var result = MapToAccount(response);

            switch (selection)
            {
                case AccountFieldsSelection.Organisations:
                    _logger.LogInformation($"Getting Organisations for the account {result.AccountId}");
                    var legalEntities = await GetLegalEntities(response.LegalEntities ?? new ResourceList(new List<ResourceViewModel>()));
                    result.LegalEntities = legalEntities;
                    break;
                case AccountFieldsSelection.TeamMembers:
                    _logger.LogInformation($"Getting TeamMembers for the account {result.AccountId}");
                    var teamMembers = await GetAccountTeamMembers(result.HashedAccountId);
                    result.TeamMembers = teamMembers;
                    break;
                case AccountFieldsSelection.PayeSchemes:
                    _logger.LogInformation($"Getting PayeSchemes for the account {result.AccountId}");
                    result.PayeSchemes = await MapToDomainPayeSchemeAsync(response);
                    return result;
                case AccountFieldsSelection.Finance:
                    _logger.LogInformation($"Getting PayeSchemes and Transactions for the account {result.AccountId}");
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
                catch (Exception e)
                {
                    _logger.LogError(e, $"Exception occured in Account API type of {nameof(TransactionsViewModel)} for period {financialYearIterator.Year}.{financialYearIterator.Month} id {accountId}");
                }
                financialYearIterator = financialYearIterator.AddMonths(1);
            }

            return GetFilteredTransactions(response);
        }

        private List<TransactionViewModel> GetFilteredTransactions(IEnumerable<TransactionViewModel> response)
        {
            return response.Where(x => x.Description != null && x.Amount != 0).OrderByDescending(x => x.DateCreated).ToList();
        }

        private async Task<IEnumerable<PayeSchemeViewModel>> GetPayeSchemes(AccountDetailViewModel response)
        {
            var payes = new List<PayeSchemeViewModel>();
            var payesBatches = response.PayeSchemes
                    .Select((item, inx) => new { item, inx })
                    .GroupBy(x => x.inx / 50)
                    .Select(g => g.Select(x => x.item));

            foreach (var payeBatch in payesBatches)
            {
                var payeTasks = response.PayeSchemes.Select(payeScheme =>
                {
                    var obscured = _payeSchemeObfuscator.ObscurePayeScheme(payeScheme.Id).Replace("/", "%252f");
                    var paye = payeScheme.Id.Replace("/", "%252f");
                    _logger.LogDebug(
                        $"IAccountApiClient.GetResource<PayeSchemeViewModel>(\"{payeScheme.Href.Replace(paye, obscured)}\");");

                    return _accountApiClient.GetResource<PayeSchemeViewModel>(payeScheme.Href).ContinueWith(payeTask =>
                    {
                        if (!payeTask.IsFaulted)
                        {
                            return payeTask.Result;
                        }
                        else
                        {
                            _logger.LogError(payeTask.Exception, $"Exception occured in Account API type of {nameof(PayeSchemeViewModel)} at {payeScheme.Href} id {payeScheme.Id}");
                            return new PayeSchemeViewModel();
                        }
                    });
                });

                payes.AddRange(await Task.WhenAll(payeTasks));
            }


            return payes.Select(payeSchemeViewModel =>
            {
                if (IsValidPayeScheme(payeSchemeViewModel))
                {
                    var item = new PayeSchemeViewModel
                    {
                        Ref = payeSchemeViewModel.Ref,
                        DasAccountId = payeSchemeViewModel.DasAccountId,
                        AddedDate = payeSchemeViewModel.AddedDate,
                        RemovedDate = payeSchemeViewModel.RemovedDate,
                        Name = payeSchemeViewModel.Name
                    };

                    return item;
                }

                return null;
            })
             .Where(x => x != null)
             .OrderBy(x => x.Ref);
        }

        private bool IsValidPayeScheme(PayeSchemeViewModel result)
        {
            return result.AddedDate <= DateTime.UtcNow &&
                   (result.RemovedDate == null || result.RemovedDate > DateTime.UtcNow);
        }

        private async Task<ICollection<TeamMemberViewModel>> GetAccountTeamMembers(string resultHashedAccountId)
        {
            try
            {
                _logger.LogDebug(
                    $"{nameof(IAccountApiClient)}.{nameof(_accountApiClient.GetAccountUsers)}(\"{resultHashedAccountId}\");");
                var teamMembers = await _accountApiClient.GetAccountUsers(resultHashedAccountId);

                return teamMembers;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Account Team Member with id {resultHashedAccountId} not found");
                return new List<TeamMemberViewModel>();
            }
        }

        private async Task<List<LegalEntityViewModel>> GetLegalEntities(ResourceList responseLegalEntities)
        {
            var legalEntitiesList = new List<LegalEntityViewModel>();

            foreach (var legalEntity in responseLegalEntities)
            {
                _logger.LogDebug(
                    $"{nameof(IAccountApiClient)}.{nameof(_accountApiClient.GetResource)}<{nameof(LegalEntityViewModel)}>(\"{legalEntity.Href}\");");
                try
                {
                    var legalResponse = await _accountApiClient.GetResource<LegalEntityViewModel>(legalEntity.Href);

                    if (legalResponse.AgreementStatus == EmployerAgreementStatus.Signed ||
                        legalResponse.AgreementStatus == EmployerAgreementStatus.Pending ||
                        legalResponse.AgreementStatus == EmployerAgreementStatus.Superseded)
                        legalEntitiesList.Add(legalResponse);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Exception occured calling Account API for type of {nameof(LegalEntityViewModel)} at {legalEntity.Href} id {legalEntity.Id}");
                }
            }

            return legalEntitiesList;
        }

        private Core.Models.Account MapToAccount(AccountDetailViewModel accountDetailViewModel)
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
}