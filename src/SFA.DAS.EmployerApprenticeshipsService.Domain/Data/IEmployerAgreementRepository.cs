using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IEmployerAgreementRepository
    {
        Task<List<LegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId);
        Task CreateEmployerAgreementTemplate(string text);
        Task<EmployerAgreementView> GetEmployerAgreement(long agreementId);
        Task SignAgreement(long agreementId, string externalUserId, string signedByName);
    }
}