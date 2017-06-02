using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.EAS.Domain.Models.Settings;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IAccountRepository
    {
        Task<CreateAccountResult> CreateAccount(long userId, string employerNumber, string employerName, string employerRegisteredAddress, DateTime? employerDateOfIncorporation, string employerRef, string accessToken, string refreshToken, string companyStatus, string employerRefName, short source, short? publicSectorDataSource, string sector);
        
        Task<List<PayeView>> GetPayeSchemesByAccountId(long accountId);
        Task RemovePayeFromAccount(long accountId, string payeRef);
        Task<EmployerAgreementView> CreateLegalEntity(long accountId, LegalEntity legalEntity);
        Task AddPayeToAccount(Paye payeScheme);
        Task<List<EmployerAgreementView>> GetEmployerAgreementsLinkedToAccount(long accountId);
        Task SetHashedId(string hashedId, long accountId);

        Task<List<UserNotificationSetting>> GetUserLegalEntitySettings(string userRef, long accountId);
        Task UpdateUserLegalEntitySettings(string userRef, long accountId, List<UserNotificationSetting> settings);
    }
}