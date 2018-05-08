using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface ILegalEntityRepository
    {
        Task<long[]> GetAccountsLinkedToLegalEntity(long legalEntityId);

        Task<long[]> GetAccountsLinkedToLegalEntityWithoutSpecificAgreement(long legalEntityId, int templateId);

        Task<LegalEntityView> GetLegalEntityById(long accountId, long id);

        Task<long[]> GetLegalEntitiesWithoutSpecificAgreement(long firstId, int count, int agreementId);
    }
}
