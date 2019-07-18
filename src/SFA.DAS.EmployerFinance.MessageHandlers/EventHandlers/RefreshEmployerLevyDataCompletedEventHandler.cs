using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class RefreshEmployerLevyDataCompletedEventHandler : IHandleMessages<RefreshEmployerLevyDataCompletedEvent>
    {
        private readonly IMessagePublisher _messagePublisher;

        public RefreshEmployerLevyDataCompletedEventHandler(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public async Task Handle(RefreshEmployerLevyDataCompletedEvent message, IMessageHandlerContext context)
        {
            await _messagePublisher.PublishAsync(
                new RefreshEmployerLevyDataCompletedMessage(
                    message.AccountId,
                    message.LevyImported,
                    message.PeriodMonth,
                    message.PeriodYear,
                    message.Created,
                    string.Empty,
                    string.Empty));
        }
    }
}
