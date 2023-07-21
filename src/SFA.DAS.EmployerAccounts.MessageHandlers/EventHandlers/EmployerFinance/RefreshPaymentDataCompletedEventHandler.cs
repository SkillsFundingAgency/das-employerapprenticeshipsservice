using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.EventHandlers.EmployerFinance;

public class RefreshPaymentDataCompletedEventHandler : IHandleMessages<RefreshPaymentDataCompletedEvent>
{
    private readonly ILegacyTopicMessagePublisher _messagePublisher;

    public RefreshPaymentDataCompletedEventHandler(ILegacyTopicMessagePublisher messagePublisher)
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