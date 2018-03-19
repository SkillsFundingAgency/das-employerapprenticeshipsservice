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

        //protected override Task OnFatalAsync(Exception ex)
        //{
        //    return base.OnFatalAsync(ex);
        //}

        //public new async Task RunAsync(CancellationTokenSource cancellationTokenSource)
        //{
        //    using (var subscriber = _subscriberFactory.GetSubscriber<PaymentProcessorQueueMessage>())
        //    {
        //        while (!cancellationTokenSource.Token.IsCancellationRequested)
        //        {
        //            IMessage<PaymentProcessorQueueMessage> message = null;

        //            Log.Debug($"Getting message of type {typeof(PaymentProcessorQueueMessage).FullName} from azure topic message queue");

        //            try
        //            {
        //                message = await subscriber.ReceiveAsAsync();
        //            }
        //            catch (Exception ex)
        //            {
        //                Log.Fatal(ex, $"Failed to retrieve message {typeof(PaymentProcessorQueueMessage).FullName}");
        //                await OnFatalAsync(ex);
        //                cancellationTokenSource.Cancel();

        //                throw;
        //            }

        //            Log.Debug($"Recieved message of type {typeof(PaymentProcessorQueueMessage).FullName} from azure topic message queue");

        //            try
        //            {
        //                if (message == null)
        //                {
        //                    Log.Debug($"No messages on queue of type {typeof(PaymentProcessorQueueMessage).FullName}");
        //                    await Task.Delay(500, cancellationTokenSource.Token);
        //                    continue;
        //                }

        //                if (message.Content == null)
        //                {
        //                    Log.Debug($"Message of type {typeof(PaymentProcessorQueueMessage).FullName} has null content");

        //                    await message.CompleteAsync();
        //                    continue;
        //                }

        //                Log.Debug($"Processing message of type {typeof(PaymentProcessorQueueMessage).FullName}");
        //                await ProcessMessage(message.Content);

        //                await message.CompleteAsync();
        //                Log.Info($"Completed message {typeof(PaymentProcessorQueueMessage).FullName}");

        //            }
        //            catch (Exception ex)
        //            {
        //                Log.Error(ex, $"Failed to process message {typeof(PaymentProcessorQueueMessage).FullName}");

        //                if (message != null && message.Content != null)
        //                {
        //                    await message.AbortAsync();
        //                }

        //                await OnErrorAsync(message, ex);
        //            }
        //        }
        //    }
        //}
    }
}
