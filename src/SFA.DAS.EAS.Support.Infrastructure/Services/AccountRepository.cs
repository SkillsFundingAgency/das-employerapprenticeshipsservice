using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using SFA.DAS.EAS.Support.Core.Services;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public sealed class AccountRepository : IAccountRepository
    {
        private readonly IAccountApiClient _accountApiClient;
        private readonly IDatetimeService _datetimeService;
        private readonly ILog _logger;
        private readonly IPayeSchemeObfuscator _payeSchemeObfuscator;
        private readonly IHashingService _hashingService;

        public AccountRepository(IAccountApiClient accountApiClient,
            IPayeSchemeObfuscator payeSchemeObfuscator,
            IDatetimeService datetimeService,
            ILog logger,
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
                _logger.Debug(
                    $"{nameof(IAccountApiClient)}.{nameof(IAccountApiClient.GetResource)}<{nameof(AccountDetailViewModel)}>(\"/api/accounts/{id.ToUpper()}\");");

                var response = await _accountApiClient.GetResource<AccountDetailViewModel>($"/api/accounts/{id.ToUpper()}");

                return await GetAdditionalFields(response, selection);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Account with id {id} not found");
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
                _logger.Warn($"The Account API Http request threw an exception while fetching Page {pageNumber} - Exception :\r\n{e}");
            }
            catch (Exception e)
            {
                _logger.Error(e,$"A general exception has been thrown while requesting employer account details");
            }

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
                _logger.Error(ex, "Exception while loading all account details");

                throw;
            }
        }

        public async Task<decimal> GetAccountBalance(string id)
        {
            try
            {
                var response = await _accountApiClient.GetResource<AccountWithBalanceViewModel>($"/api/accounts/{id}");

                _logger.Debug(
                    $"{nameof(IAccountApiClient)}.{nameof(_accountApiClient.GetResource)}<{nameof(AccountWithBalanceViewModel)}>(\"/api/accounts/{id}\"); {response.Balance}");

                return response.Balance;
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Account Balance with id {id} not found");
                return 0;
            }
        }

        private async Task<IEnumerable<Core.Models.Account>> GetAccountSearchDetails(IEnumerable<AccountWithBalanceViewModel> accounts)
        {
            var accountsWithDetails = new List<Core.Models.Account>();

            foreach (var account in accounts)
                try
                {
                    var accountViewModel = await _accountApiClient.GetAccount(account.AccountHashId);
                    var accountWithDetails = await GetAdditionalFields(accountViewModel, AccountFieldsSelection.PayeSchemes);
                    accountsWithDetails.Add(accountWithDetails);
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Exception while retrieving details for account ID {account.AccountHashId}");
                }

            return accountsWithDetails;
        }

        private async Task<Core.Models.Account> GetAdditionalFields(AccountDetailViewModel response, AccountFieldsSelection selection)
        {
            var result = MapToAccount(response);

            switch (selection)
            {
                case AccountFieldsSelection.Organisations:
                    var legalEntities = await GetLegalEntities(response.LegalEntities ?? new ResourceList(new List<ResourceViewModel>()));
                    result.LegalEntities = legalEntities;
                    break;
                case AccountFieldsSelection.TeamMembers:
                    var teamMembers = await GetAccountTeamMembers(result.HashedAccountId);
                    result.TeamMembers = teamMembers;
                    break;
                case AccountFieldsSelection.PayeSchemes:
                    result.PayeSchemes = await MapToDomainPayeSchemeAsync(response);
                    return result;
                case AccountFieldsSelection.Finance:
                    result.PayeSchemes = await MapToDomainPayeSchemeAsync(response);
                    result.Transactions = await GetAccountTransactions(response.HashedAccountId); 
                    return result;
            }

            return result;
        }

        private async Task<List<TransactionViewModel>> GetAccountTransactions(string accountId)
        {
            var endDate = DateTime.Now;
            var financialYearIterator = _datetimeService.GetBeginningFinancialYear(endDate);
            var response = new List<TransactionViewModel>();

            while (financialYearIterator <= endDate)
            {
                try
                {
                    var transactions = await _accountApiClient.GetTransactions(accountId, financialYearIterator.Year,
                        financialYearIterator.Month);

                    response.AddRange(transactions);
                    financialYearIterator = financialYearIterator.AddMonths(1);
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Exception occured in Account API type of {nameof(TransactionsViewModel)} for period {financialYearIterator.Year}.{financialYearIterator.Month} id {accountId}");
                }
            }

            return GetFilteredTransactions(response);
        }

        private List<TransactionViewModel> GetFilteredTransactions(IEnumerable<TransactionViewModel> response)
        {
            return response.Where(x => x.Description != null && x.Amount != 0).OrderByDescending(x => x.DateCreated).ToList();
        }

        private async Task<IEnumerable<PayeSchemeViewModel>> GetPayeSchemes(AccountDetailViewModel response)
        {
            var result = new List<PayeSchemeViewModel>();
            foreach (var payeScheme in response.PayeSchemes ?? new ResourceList(new List<ResourceViewModel>()))
            {
                var obscured = _payeSchemeObfuscator.ObscurePayeScheme(payeScheme.Id).Replace("/", "%252f");
                var paye = payeScheme.Id.Replace("/", "%252f");
                _logger.Debug(
                    $"IAccountApiClient.GetResource<PayeSchemeViewModel>(\"{payeScheme.Href.Replace(paye, obscured)}\");");
                try
                {
                    var payeSchemeViewModel = await _accountApiClient.GetResource<PayeSchemeViewModel>(payeScheme.Href);

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
                        result.Add(item);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Exception occured in Account API type of {nameof(LegalEntityViewModel)} at  {payeScheme.Href} id {payeScheme.Id}");
                }
            }

            return result.OrderBy(x => x.Ref);
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
                _logger.Debug(
                    $"{nameof(IAccountApiClient)}.{nameof(_accountApiClient.GetAccountUsers)}(\"{resultHashedAccountId}\");");
                var teamMembers = await _accountApiClient.GetAccountUsers(resultHashedAccountId);

                return teamMembers;
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Account Team Member with id {resultHashedAccountId} not found");
                return new List<TeamMemberViewModel>();
            }
        }

        private async Task<List<LegalEntityViewModel>> GetLegalEntities(ResourceList responseLegalEntities)
        {
            var legalEntitiesList = new List<LegalEntityViewModel>();

            foreach (var legalEntity in responseLegalEntities)
            {
                _logger.Debug(
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
                    _logger.Error(ex, $"Exception occured calling Account API for type of {nameof(LegalEntityViewModel)} at {legalEntity.Href} id {legalEntity.Id}");
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
                DateRegistered = accountDetailViewModel.DateRegistered,
                OwnerEmail = accountDetailViewModel.OwnerEmail
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