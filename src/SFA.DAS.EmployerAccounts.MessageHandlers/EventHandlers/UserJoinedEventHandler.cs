using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers;

public class UserJoinedEventHandler : IHandleMessages<UserJoinedEvent>
{
    private readonly ILegacyTopicMessagePublisher _messagePublisher;

    public UserJoinedEventHandler(ILegacyTopicMessagePublisher messagePublisher)
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