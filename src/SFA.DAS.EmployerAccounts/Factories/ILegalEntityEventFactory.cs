using SFA.DAS.EmployerAccounts.Models.LegalEntity;

namespace SFA.DAS.EmployerAccounts.Factories
{
    public interface ILegalEntityEventFactory
    {
        LegalEntityCreatedEvent CreateLegalEntityCreatedEvent(string hashedAccountId, long legalEntityId);
    }
}
