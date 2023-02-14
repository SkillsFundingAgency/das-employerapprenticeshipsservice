using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using RefreshPaymentDataCompletedEvent = SFA.DAS.EmployerFinance.Messages.Events.RefreshPaymentDataCompletedEvent;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers.EmployerFinance;

public class RefreshPaymentDataCompletedEventHandler : IHandleMessages<RefreshPaymentDataCompletedEvent>
{
    private readonly IMessagePublisher _messagePublisher;

    public RefreshPaymentDataCompletedEventHandler(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;
    }

    public async Task Handle(RefreshPaymentDataCompletedEvent message, IMessageHandlerContext context)
    {
        await _messagePublisher.PublishAsync(
            new RefreshPaymentDataCompletedMessage(
                message.AccountId,
                message.PaymentsProcessed,
                message.Created,
                message.PeriodEnd,
                string.Empty,
                string.Empty));
    }
}