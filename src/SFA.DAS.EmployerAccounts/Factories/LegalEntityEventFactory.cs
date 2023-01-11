using SFA.DAS.EmployerAccounts.Events.LegalEntity;

namespace SFA.DAS.EmployerAccounts.Factories;

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