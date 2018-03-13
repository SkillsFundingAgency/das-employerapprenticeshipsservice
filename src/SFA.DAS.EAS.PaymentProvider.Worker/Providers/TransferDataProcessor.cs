using MediatR;
using SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker.Providers
{
    public class TransferDataProcessor : MessageProcessor<PaymentProcessorQueueMessage>
    {
        private readonly ILog _logger;
        private readonly IMediator _mediator;

        public TransferDataProcessor(IMessageSubscriberFactory subscriberFactory, ILog logger, IMediator mediator) 
            : base(subscriberFactory, logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(PaymentProcessorQueueMessage message)
        {
            _logger.Info($"Processing refresh account transfers command for AccountId:{message.AccountId} PeriodEnd:{message.PeriodEndId}");

            await _mediator.SendAsync(new RefreshAccountTransfersCommand
            {
                AccountId = message.AccountId,
                PeriodEnd = message.PeriodEndId
            });
        }
    }
}
