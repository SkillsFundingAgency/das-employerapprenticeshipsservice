using MediatR;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker.Providers
{
    [TopicSubscription("MA_PaymentDataProcessor")]
    public class PaymentDataProcessor : IPaymentDataProcessor
    {
        private readonly IMessageSubscriberFactory _subscriberFactory;
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public PaymentDataProcessor(IMessageSubscriberFactory subscriberFactory, IMediator mediator, ILog logger)
        {
            _subscriberFactory = subscriberFactory;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            using (var subscriber = _subscriberFactory.GetSubscriber<PaymentProcessorQueueMessage>())
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var message = await subscriber.ReceiveAsAsync();

                    try
                    {
                        await ProcessMessage(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.Fatal(ex,
                            $"Refresh payment message processing failed for account with ID [{message?.Content?.AccountId}]");
                        break; //Stop processing anymore messages as this failure needs to be investigated
                    }
                }
            }
        }

        private async Task ProcessMessage(IMessage<PaymentProcessorQueueMessage> message)
        {
            if (message?.Content?.AccountId == null)
            {
                if (message != null)
                {
                    await message.CompleteAsync();
                }

                return;
            }

            _logger.Info($"Processing refresh payment command for AccountId='{message.Content.AccountId}' and  PeriodEnd = '{message.Content.PeriodEndId}'");

            await _mediator.SendAsync(new RefreshPaymentDataCommand
            {
                AccountId = message.Content.AccountId,
                PeriodEnd = message.Content.PeriodEndId,
                PaymentUrl = message.Content.AccountPaymentUrl
            });

            _logger.Info($"Processing refresh account transfers command for AccountId:{message.Content.AccountId} PeriodEnd:{message.Content.PeriodEndId}");


            await _mediator.SendAsync(new RefreshAccountTransfersCommand
            {
                AccountId = message.Content.AccountId,
                PeriodEnd = message.Content.PeriodEndId
            });

            await message.CompleteAsync();
        }
    }
}
