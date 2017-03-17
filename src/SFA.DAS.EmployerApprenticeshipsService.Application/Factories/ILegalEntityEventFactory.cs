using SFA.DAS.EAS.Account.Api.Types.Events;

namespace SFA.DAS.EAS.Application.Factories
{
    public interface ILegalEntityEventFactory
    {
        LegalEntityCreatedEvent CreateLegalEntityCreatedEvent(string hashedAccountId, long legalEntityId);
    }
}
