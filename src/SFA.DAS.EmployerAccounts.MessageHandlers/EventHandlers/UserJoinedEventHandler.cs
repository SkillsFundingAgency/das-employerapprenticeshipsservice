using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class UserJoinedEventHandler : IHandleMessages<UserJoinedEvent>
{
    private readonly IEventPublisher _messagePublisher;

    public UserJoinedEventHandler(IEventPublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }
    public async Task Handle(UserJoinedEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.Publish(
            new UserJoinedMessage(
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));
    }
}