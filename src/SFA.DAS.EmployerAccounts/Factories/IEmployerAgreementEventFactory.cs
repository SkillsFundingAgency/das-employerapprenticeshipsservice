using SFA.DAS.EmployerAccounts.Events.Agreement;

namespace SFA.DAS.EmployerAccounts.Factories;

public interface IEmployerAgreementEventFactory
{
    AgreementSignedEvent CreateSignedEvent(string hashedAccountId, string hashedLegalEntityId, string hashedAgreementId);
    AgreementRemovedEvent RemoveAgreementEvent(string hashedAgreementId);
}