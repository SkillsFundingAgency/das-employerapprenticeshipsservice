using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IEmployerAgreementRepository
    {
        Task<List<LegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId, bool signedOnly);
        Task<EmployerAgreementView> GetEmployerAgreement(long agreementId);

        Task SignAgreement(SignEmployerAgreement agreement);
        Task CreateEmployerAgreementTemplate(string templateRef, string text);
        Task<EmployerAgreementTemplate> GetEmployerAgreementTemplate(int templateId);
        Task<EmployerAgreementTemplate> GetLatestAgreementTemplate();
        Task<EmployerAgreementView> GetLatestAccountLegalEntityAgreement(long accountId, long legalEntityId);
        Task RemoveLegalEntityFromAccount(long agreementId);
    }
}