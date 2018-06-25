using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
    public class ChangedAccountNameEventHandler : IHandleMessages<IChangedAccountNameEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public ChangedAccountNameEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(IChangedAccountNameEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new AccountNameChangedMessage(
                message.PreviousName,
                message.CurrentName,
                message.AccountId,
                message.UserName,
                message.UserRef.ToString()));
        }
    }
}
