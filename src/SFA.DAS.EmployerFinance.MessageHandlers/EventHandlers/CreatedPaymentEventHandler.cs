using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
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