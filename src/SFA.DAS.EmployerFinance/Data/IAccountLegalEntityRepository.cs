using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IAccountLegalEntityRepository
    {
        Task CreateAccountLegalEntity(long id, DateTime? deleted, long? pendingAgreementId, long? signedAgreementId,
            int? signedAgreementVersion, long accountId, long legalEntityId);
    }
}