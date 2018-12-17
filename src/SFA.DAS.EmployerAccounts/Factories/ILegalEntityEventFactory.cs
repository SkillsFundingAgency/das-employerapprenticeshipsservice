using SFA.DAS.EAS.Account.Api.Types.Events.LegalEntity;

namespace SFA.DAS.EmployerAccounts.Factories
{
    public interface ILegalEntityEventFactory
    {
        LegalEntityCreatedEvent CreateLegalEntityCreatedEvent(string hashedAccountId, long legalEntityId);
    }
}
