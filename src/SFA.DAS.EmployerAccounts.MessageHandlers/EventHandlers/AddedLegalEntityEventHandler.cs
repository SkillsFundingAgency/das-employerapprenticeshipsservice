using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class AddedLegalEntityEventHandler : IHandleMessages<AddedLegalEntityEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public AddedLegalEntityEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(AddedLegalEntityEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.PublishAsync(
            new LegalEntityAddedMessage(
                message.AccountId,
                message.AgreementId,
                message.OrganisationName,
                message.LegalEntityId,
                message.UserName,
                message.UserRef.ToString()));
    }
}