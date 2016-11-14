using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.PaymentProvider.Worker.Providers
{
    public class PaymentDataProcessor : IPaymentDataProcessor
    {
        [QueueName]
        public string refresh_payments { get; set; }

        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IMediator _mediator;

        public PaymentDataProcessor(IPollingMessageReceiver pollingMessageReceiver, IMediator mediator)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
        }

        public async Task Handle()
        {
            var message = await _pollingMessageReceiver.ReceiveAsAsync<PaymentProcessorQueueMessage>();

            if(message?.Content?.AccountId != null)
            {
                await _mediator.SendAsync(new RefreshPaymentDataCommand
                {
                    AccountId = message.Content.AccountId,
                    PeriodEnd = message.Content.PeriodEndId,
                    PaymentUrl = message.Content.AccountPaymentUrl
                });
            }

            if (message != null)
            {
                await message.CompleteAsync();
            }
        }
    }
}
