using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IEmployerAgreementRepository
    {
        Task<List<LegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId);
        Task<EmployerAgreementView> GetEmployerAgreement(long agreementId);
        Task SignAgreement(long agreementId, long signedById, string signedByName, DateTime signedDate);
        Task ReleaseEmployerAgreementTemplate(int templateId);
        Task CreateEmployerAgreementTemplate(string templateRef, string text);
        Task<EmployerAgreementTemplate> GetEmployerAgreementTemplate(int templateId);
        Task<EmployerAgreementTemplate> GetLatestAgreementTemplate();
    }
}