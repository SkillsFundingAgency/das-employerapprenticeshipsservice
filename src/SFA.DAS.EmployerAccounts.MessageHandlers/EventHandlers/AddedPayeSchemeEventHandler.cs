using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class AddedPayeSchemeEventHandler : IHandleMessages<AddedPayeSchemeEvent>
{
    private readonly IEventPublisher _messagePublisher;

    public AddedPayeSchemeEventHandler(IEventPublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(AddedPayeSchemeEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.Publish(
            new PayeSchemeAddedMessage(
                message.PayeRef,
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));
    }
}