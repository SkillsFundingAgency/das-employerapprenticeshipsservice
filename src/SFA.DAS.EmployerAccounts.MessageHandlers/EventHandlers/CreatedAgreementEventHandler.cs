using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class CreatedAgreementEventHandler : IHandleMessages<CreatedAgreementEvent>
{
    private readonly IEventPublisher _messagePublisher;

    public CreatedAgreementEventHandler(IEventPublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }
    public async Task Handle(CreatedAgreementEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.Publish(new AgreementCreatedMessage(
            message.AccountId,
            message.AgreementId,
            message.OrganisationName,
            message.LegalEntityId,
            message.UserName,
            message.UserRef.ToString()));
    }
}