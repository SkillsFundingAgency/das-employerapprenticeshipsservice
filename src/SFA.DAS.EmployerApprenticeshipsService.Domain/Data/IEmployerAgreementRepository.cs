using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IEmployerAgreementRepository
    {
        Task<List<LegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId);
        Task<EmployerAgreementView> GetEmployerAgreement(long agreementId);
        Task SignAgreement(long agreementId, long signedById, string signedByName);
        Task ReleaseEmployerAgreementTemplate(int templateId);
        Task CreateEmployerAgreementTemplate(string templateRef, string text);
        Task<EmployerAgreementTemplate> GetEmployerAgreementTemplate(int templateId);
        Task<EmployerAgreementTemplate> GetLatestAgreementTemplate();
    }
}