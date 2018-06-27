using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
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
}
