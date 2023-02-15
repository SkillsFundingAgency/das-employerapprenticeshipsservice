using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class AddedPayeSchemeEventHandler : IHandleMessages<AddedPayeSchemeEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public AddedPayeSchemeEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(AddedPayeSchemeEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.PublishAsync(
            new PayeSchemeAddedMessage(
                message.PayeRef,
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));
    }
}