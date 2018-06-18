using MediatR;
using SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System;
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
            ILog logger,
            IMessageContextProvider messageContextProvider)
            : base(subscriberFactory, logger, messageContextProvider)
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
                ReceiverAccountId = message.AccountId,
                PeriodEnd = message.PeriodEnd
            });

            await _messagePublisher.PublishAsync(new AccountTransfersProcessingCompletedMessage
            {
                AccountId = message.AccountId,
                PeriodEnd = message.PeriodEnd
            });
        }

        protected override Task OnErrorAsync(IMessage<AccountPaymentsProcessingCompletedMessage> message, Exception ex)
        {
            _logger.Error(ex, $"Could not process payment processing completed message for Account Id {message.Content.AccountId} & period end {message.Content.PeriodEnd}");
            return Task.CompletedTask;
        }

        protected override Task OnFatalAsync(Exception ex)
        {
            _logger.Fatal(ex, "Failed to process payment processing completed message");
            return Task.CompletedTask;
        }
    }
}
