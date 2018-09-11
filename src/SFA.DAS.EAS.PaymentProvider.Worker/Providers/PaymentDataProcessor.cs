using MediatR;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker.Providers
{
    [TopicSubscription("MA_PaymentDataProcessor")]
    public class PaymentDataProcessor : MessageProcessor<PaymentProcessorQueueMessage>
    {
        private readonly IMessageSubscriberFactory _subscriberFactory;
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public PaymentDataProcessor(IMessageSubscriberFactory subscriberFactory, IMediator mediator, ILog logger)
            : base(subscriberFactory, logger)
        {
            _subscriberFactory = subscriberFactory;
            _mediator = mediator;
            _logger = logger;
        }

        protected override async Task ProcessMessage(PaymentProcessorQueueMessage message)
        {
            _logger.Info($"Processing refresh payment command for AccountId:{message.AccountId} PeriodEnd:{message.PeriodEndId}");

            await _mediator.SendAsync(new RefreshPaymentDataCommand
            {
                AccountId = message.AccountId,
                PeriodEnd = message.PeriodEndId,
                PaymentUrl = message.AccountPaymentUrl
            });
        }

        protected override Task OnErrorAsync(IMessage<PaymentProcessorQueueMessage> message, Exception ex)
        {
            _logger.Error(ex, $"Could not process payment processor message for Account Id {message?.Content?.AccountId} & period end {message?.Content?.PeriodEndId}");
            return Task.CompletedTask;
        }

        protected override Task OnFatalAsync(Exception ex)
        {
            _logger.Fatal(ex, "Failed to process payment processor message");
            return Task.CompletedTask;
        }
    }
}
