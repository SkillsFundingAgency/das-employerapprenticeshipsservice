using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
    public class InvitedUserEventHandler : IHandleMessages<InvitedUserEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public InvitedUserEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(InvitedUserEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(
                new UserInvitedMessage(
                    message.PersonInvited,
                    message.AccountId,
                    message.UserName,
                    message.UserRef.ToString()));
        }
    }
}
