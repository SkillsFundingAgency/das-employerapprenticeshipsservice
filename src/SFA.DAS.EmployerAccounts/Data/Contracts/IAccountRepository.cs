using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data.Contracts;

public interface IAccountRepository
{
    Task<CreateAccountResult> CreateAccount(CreateAccountParams createParams);
    Task<CreateUserAccountResult> CreateUserAccount(long userId, string employerName);
    Task<EmployerAgreementView> CreateLegalEntityWithAgreement(CreateLegalEntityWithAgreementParams createParams);
    Task<List<PayeView>> GetPayeSchemesByAccountId(long accountId);
    Task<List<UserNotificationSetting>> GetUserAccountSettings(string userRef);
    Task UpdateAccountHashedIds(long accountId, string hashedId, string publicHashedId);
    Task UpdateUserAccountSettings(string userRef, List<UserNotificationSetting> settings);
    Task UpdateLegalEntityDetailsForAccount(long accountLegalEntityId, string name, string address);
}