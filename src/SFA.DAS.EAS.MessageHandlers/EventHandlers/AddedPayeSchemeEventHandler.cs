using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
    public class AddedPayeSchemeEventHandler : IHandleMessages<IAddedPayeSchemeEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public AddedPayeSchemeEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(IAddedPayeSchemeEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(new PayeSchemeAddedMessage(message.PayeRef, message.AccountId, message.UserName, message.UserRef.ToString()));
        }
    }
}
