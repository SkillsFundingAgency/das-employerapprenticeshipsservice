using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface ILegalEntityRepository
    {
        Task<long[]> GetAccountsLinkedToLegalEntity(long legalEntityId);
        Task<long[]> GetAccountsLinkedToLegalEntityWithoutSpecificAgreement(long legalEntityId, int templateId);
        Task<long[]> GetLegalEntitiesWithoutSpecificAgreement(long firstId, int count, int agreementId);
    }
}
