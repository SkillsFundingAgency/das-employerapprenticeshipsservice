using SFA.DAS.EAS.Account.Api.Types.Events.Agreement;

namespace SFA.DAS.EAS.Application.Factories
{
    public interface IEmployerAgreementEventFactory
    {
        AgreementSignedEvent CreateSignedEvent(string hashedAccountId, string hashedLegalEntityId, string hashedAgreementId);
        AgreementRemovedEvent RemoveAgreementEvent(string hashedAgreementId);
    }
}
