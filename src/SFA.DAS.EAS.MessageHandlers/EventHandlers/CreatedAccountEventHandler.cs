using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
    public class CreatedAccountEventHandler : IHandleMessages<ICreatedAccountEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public CreatedAccountEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(ICreatedAccountEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new AccountCreatedMessage(message.AccountId, message.UserName, message.UserRef.ToString()));
        }
    }
}
