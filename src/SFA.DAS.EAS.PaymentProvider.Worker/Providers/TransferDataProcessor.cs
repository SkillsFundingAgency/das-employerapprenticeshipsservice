using MediatR;
using SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker.Providers
{
    [TopicSubscription("MA_TransferDataProcessor")]
    public class TransferDataProcessor : MessageProcessor<AccountPaymentsProcessingCompletedMessage>
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILog _logger;
        private readonly IMediator _mediator;


        public TransferDataProcessor(
            IMessageSubscriberFactory subscriberFactory,
            IMessagePublisher messagePublisher,
            IMediator mediator,
            ILog logger)
            : base(subscriberFactory, logger)
        {
            _messagePublisher = messagePublisher;
            _logger = logger;
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(AccountPaymentsProcessingCompletedMessage message)
        {
            _logger.Info($"Processing refresh account transfers command for AccountId:{message.AccountId} PeriodEnd:{message.PeriodEnd}");

            await _mediator.SendAsync(new RefreshAccountTransfersCommand
            {
                AccountId = message.AccountId,
                PeriodEnd = message.PeriodEnd
            });

            await _messagePublisher.PublishAsync(new AccountTransfersProcessingCompletedMessage
            {
                AccountId = message.AccountId,
                PeriodEnd = message.PeriodEnd
            });
        }
    }
}
