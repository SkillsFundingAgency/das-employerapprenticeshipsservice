using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IEmployerAgreementRepository
    {
        Task<List<LegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId);
    }
}