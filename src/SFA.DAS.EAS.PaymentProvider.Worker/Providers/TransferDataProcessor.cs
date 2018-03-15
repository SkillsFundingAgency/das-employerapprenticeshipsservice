using MediatR;
using SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker.Providers
{
    [TopicSubscription("MA_TransferDataProcessor")]
    public class TransferDataProcessor : MessageProcessor<AccountPaymentsProcessingFinishedMessage>
    {
        private readonly ILog _logger;
        private readonly IMediator _mediator;


        public TransferDataProcessor(IMessageSubscriberFactory subscriberFactory, IMediator mediator, ILog logger)
            : base(subscriberFactory, logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        protected override async Task ProcessMessage(AccountPaymentsProcessingFinishedMessage message)
        {
            _logger.Info($"Processing refresh account transfers command for AccountId:{message.AccountId} PeriodEnd:{message.PeriodEnd}");

            await _mediator.SendAsync(new RefreshAccountTransfersCommand
            {
                AccountId = message.AccountId,
                PeriodEnd = message.PeriodEnd
            });
        }
    }
}
