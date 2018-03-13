using MediatR;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker.Providers
{
    [TopicSubscription("MA_PaymentDataProcessor")]
    public class PaymentDataProcessor : MessageProcessor<PaymentProcessorQueueMessage>
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public PaymentDataProcessor(IMessageSubscriberFactory subscriberFactory, IMediator mediator, ILog logger)
            : base(subscriberFactory, logger)
        {
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
    }
}
