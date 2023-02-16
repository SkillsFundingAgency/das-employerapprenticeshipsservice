using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class RemovedLegalEntityEventHandler : IHandleMessages<RemovedLegalEntityEvent>
{
    private readonly IEventPublisher _messagePublisher;

    public RemovedLegalEntityEventHandler(IEventPublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(RemovedLegalEntityEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.Publish(
            new LegalEntityRemovedMessage(
                message.AccountId,
                message.AgreementId,
                message.AgreementSigned,
                message.LegalEntityId,
                message.OrganisationName,
                message.UserName,
                message.UserRef.ToString()));
    }
}