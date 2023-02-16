using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class AddedLegalEntityEventHandler : IHandleMessages<AddedLegalEntityEvent>
{
    private readonly IEventPublisher _messagePublisher;

    public AddedLegalEntityEventHandler(IEventPublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(AddedLegalEntityEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.Publish(
            new LegalEntityAddedMessage(
                message.AccountId,
                message.AgreementId,
                message.OrganisationName,
                message.LegalEntityId,
                message.UserName,
                message.UserRef.ToString()));
    }
}