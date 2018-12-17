using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IAccountRepository
    {
        Task AddPayeToAccount(Paye payeScheme);
        Task<CreateAccountResult>  CreateAccount(long userId, string employerNumber, string employerName, string employerRegisteredAddress, DateTime? employerDateOfIncorporation, string employerRef, string accessToken, string refreshToken, string companyStatus, string employerRefName, short source, short? publicSectorDataSource, string sector);
        Task<EmployerAgreementView> CreateLegalEntityWithAgreement(CreateLegalEntityWithAgreementParams createParams);
        Task<AccountStats> GetAccountStats(long accountId);
        Task<List<PayeView>> GetPayeSchemesByAccountId(long accountId);
        Task<List<UserNotificationSetting>> GetUserAccountSettings(string userRef);
        Task RemovePayeFromAccount(long accountId, string payeRef);
        Task UpdateAccountHashedIds(long accountId, string hashedId, string publicHashedId);
        Task UpdateUserAccountSettings(string userRef, List<UserNotificationSetting> settings);
        Task<string> GetAccountName(long accountId);
        Task<Dictionary<long, string>> GetAccountNames(IEnumerable<long> accountIds);
        Task UpdateLegalEntityDetailsForAccount(long accountLegalEntityId, string name, string address);
        Task UpdateAccountLegalEntityPublicHashedId(long accountLegalEntityId);
        Task<long[]> GetAccountLegalEntitiesWithoutPublicHashId(long firstId, int count);
        Task<EmployerAccountOutput> GetAccountDetailsAsync(string accountName);

    }
}