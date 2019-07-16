using SFA.DAS.EmployerAccounts.Events.LegalEntity;

namespace SFA.DAS.EAS.Application.Factories
{
    public interface ILegalEntityEventFactory
    {
        LegalEntityCreatedEvent CreateLegalEntityCreatedEvent(string hashedAccountId, long legalEntityId);
    }
}
