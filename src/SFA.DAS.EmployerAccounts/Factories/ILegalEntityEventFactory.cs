using SFA.DAS.EmployerAccounts.Events.LegalEntity;

namespace SFA.DAS.EmployerAccounts.Factories;

public interface ILegalEntityEventFactory
{
    LegalEntityCreatedEvent CreateLegalEntityCreatedEvent(string hashedAccountId, long legalEntityId);
}