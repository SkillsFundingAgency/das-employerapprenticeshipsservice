using SFA.DAS.EAS.Account.Api.Types.Events.LegalEntity;

namespace SFA.DAS.EAS.Application.Factories
{
    public class LegalEntityEventFactory : ILegalEntityEventFactory
    {
        public LegalEntityCreatedEvent CreateLegalEntityCreatedEvent(string hashedAccountId, long legalEntityId)
        {
            return new LegalEntityCreatedEvent
            {
                ResourceUri = $"api/accounts/{hashedAccountId}/legalentities/{legalEntityId}"
            };
        }
    }
}
