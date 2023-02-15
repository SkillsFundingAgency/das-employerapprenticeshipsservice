using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class CreatedAgreementEventHandler : IHandleMessages<CreatedAgreementEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public CreatedAgreementEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }
    public async Task Handle(CreatedAgreementEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.PublishAsync(new AgreementCreatedMessage(
            message.AccountId,
            message.AgreementId,
            message.OrganisationName,
            message.LegalEntityId,
            message.UserName,
            message.UserRef.ToString()));
    }
}