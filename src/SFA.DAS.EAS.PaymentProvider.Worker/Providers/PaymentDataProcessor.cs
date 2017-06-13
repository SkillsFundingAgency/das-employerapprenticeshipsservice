using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.PaymentProvider.Worker.Providers
{
    public class PaymentDataProcessor : IPaymentDataProcessor
    {
        [QueueName]
        public string refresh_payments { get; set; }

        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public PaymentDataProcessor(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator, ILog logger)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await _pollingMessageReceiver.ReceiveAsAsync<PaymentProcessorQueueMessage>();

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

        private async Task ProcessMessage(Message<PaymentProcessorQueueMessage> message)
        {
            if (message?.Content?.AccountId == null)
            {
                if (message != null)
                {
                    await message.CompleteAsync();
                }

                return;
            }

            _logger.Info($"Processing refresh payment command for AccountId:{message.Content.AccountId} PeriodEnd:{message.Content.PeriodEndId}");

            await _mediator.SendAsync(new RefreshPaymentDataCommand
            {
                AccountId = message.Content.AccountId,
                PeriodEnd = message.Content.PeriodEndId,
                PaymentUrl = message.Content.AccountPaymentUrl
            });

            await message.CompleteAsync();
        }
    }
}
