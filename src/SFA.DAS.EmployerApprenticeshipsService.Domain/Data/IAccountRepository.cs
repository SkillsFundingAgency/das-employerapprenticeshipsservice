using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Account;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IAccountRepository
    {
        Task<CreateAccountResult> CreateAccount(long userId, string employerNumber, string employerName, string employerRegisteredAddress, DateTime? employerDateOfIncorporation, string employerRef, string accessToken, string refreshToken, string companyStatus, string employerRefName, short source, short? publicSectorDataSource);
        
        Task<List<PayeView>> GetPayeSchemesByAccountId(long accountId);
        Task RemovePayeFromAccount(long accountId, string payeRef);
        Task<EmployerAgreementView> CreateLegalEntity(long accountId, LegalEntity legalEntity, bool signAgreement, DateTime signedDate, long signedById);
        Task AddPayeToAccount(Paye payeScheme);
        Task<List<EmployerAgreementView>> GetEmployerAgreementsLinkedToAccount(long accountId);
        Task SetHashedId(string hashedId, long accountId);
    }
}