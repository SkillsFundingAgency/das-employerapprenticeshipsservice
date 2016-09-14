using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IAccountRepository
    {
        Task<long> CreateAccount(long userId, string employerNumber, string employerName, string employerRegisteredAddress, DateTime employerDateOfIncorporation, string employerRef, string accessToken, string refreshToken);
        Task<List<PayeView>> GetPayeSchemes(long accountId);
        Task AddPayeToAccountForExistingLegalEntity(long accountId, long legalEntityId, string employerRef, string accessToken, string refreshToken);
        Task RemovePayeFromAccount(long accountId, string payeRef);
        Task AddPayeToAccountForNewLegalEntity(Paye payeScheme, LegalEntity legalEntity);
        Task<List<EmployerAgreementView>> GetEmployerAgreementsLinkedToAccount(long accountId);
    }
}