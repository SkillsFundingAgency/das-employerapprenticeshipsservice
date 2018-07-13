using MediatR;
using SFA.DAS.EAS.Application.Commands.CreateTransferTransactions;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker.Providers
{
    [TopicSubscription("MA_TransferTransactionProcessor")]
    public class TransferTransactionProcessor : MessageProcessor<AccountTransfersProcessingCompletedMessage>
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public TransferTransactionProcessor(IMessageSubscriberFactory subscriberFactory, IMediator mediator, ILog logger, IMessageContextProvider messageContextProvider)
            : base(subscriberFactory, logger, messageContextProvider)
        {
            _mediator = mediator;
            _logger = logger;
        }

        protected override async Task ProcessMessage(AccountTransfersProcessingCompletedMessage message)
        {
            _logger.Info($"Processing create account transfer transactions command for AccountId:{message.AccountId} PeriodEnd:{message.PeriodEnd}");

            await _mediator.SendAsync(new CreateTransferTransactionsCommand
            {
                ReceiverAccountId = message.AccountId,
                PeriodEnd = message.PeriodEnd
            });
        }

        protected override Task OnErrorAsync(IMessage<AccountTransfersProcessingCompletedMessage> message, Exception ex)
        {
            _logger.Error(ex, $"Could not process account transfer processing completed message for Account Id {message.Content.AccountId} & period end {message.Content.PeriodEnd}");
            return Task.CompletedTask;
        }

        protected override Task OnFatalAsync(Exception ex)
        {
            _logger.Fatal(ex, "Failed to process account transfer processing completed message");
            return Task.CompletedTask;
        }
    }
}
