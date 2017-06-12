using SFA.DAS.EAS.Account.Api.Types.Events.Agreement;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.Factories
{
    public class EmployerAgreementEventFactory : IEmployerAgreementEventFactory
    {
        public AgreementSignedEvent CreateSignedEvent(string hashedAccountId, string hashedLegalEntityId, string hashedAgreementId)
        {
           return new AgreementSignedEvent
           {
               ResourceUrl = $"api/accounts/{hashedAccountId}/legalEntities/{hashedLegalEntityId}/agreements/{hashedAgreementId}"
           };
        }

        public AgreementRemovedEvent RemoveAgreementEvent(string hashedAgreementId)
        {
            return new AgreementRemovedEvent {HashedAgreementId = hashedAgreementId};
        }
    }
}
