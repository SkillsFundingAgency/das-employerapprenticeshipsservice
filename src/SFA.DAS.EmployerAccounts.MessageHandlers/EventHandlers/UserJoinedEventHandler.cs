using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class UserJoinedEventHandler : IHandleMessages<UserJoinedEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public UserJoinedEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }
    public async Task Handle(UserJoinedEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.PublishAsync(
            new UserJoinedMessage(
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));
    }
}