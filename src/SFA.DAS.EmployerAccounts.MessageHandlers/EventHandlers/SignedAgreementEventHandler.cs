using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class SignedAgreementEventHandler : IHandleMessages<SignedAgreementEvent>
{
    private readonly IEventPublisher _messagePublisher;

    public SignedAgreementEventHandler(IEventPublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(SignedAgreementEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.Publish(new AgreementSignedMessage(
            message.AccountId,
            message.AgreementId,
            message.OrganisationName,
            message.LegalEntityId,
            message.CohortCreated,
            message.UserName,
            message.UserRef.ToString()));
    }
}