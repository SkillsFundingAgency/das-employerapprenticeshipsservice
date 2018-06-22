using NServiceBus;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.EventHandlers
{
    public class CreatedPaymentEventHandler : IHandleMessages<CreatedPaymentEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public CreatedPaymentEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(CreatedPaymentEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(
                new PaymentCreatedMessage(
                    message.ProviderName,
                    message.Amount,
                    message.AccountId,
                    string.Empty,
                    string.Empty));
        }
    }
}
