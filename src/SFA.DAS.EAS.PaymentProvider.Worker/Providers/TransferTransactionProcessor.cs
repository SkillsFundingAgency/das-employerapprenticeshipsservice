using MediatR;
using SFA.DAS.EAS.Application.Commands.CreateTransferTransactions;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker.Providers
{
    public class TransferTransactionProcessor : MessageProcessor<AccountTransfersProcessingCompletedMessage>
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public TransferTransactionProcessor(IMessageSubscriberFactory subscriberFactory, IMediator mediator, ILog logger)
            : base(subscriberFactory, logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        protected override async Task ProcessMessage(AccountTransfersProcessingCompletedMessage message)
        {
            _logger.Info($"Processing create account transfer transactions command for AccountId:{message.AccountId} PeriodEnd:{message.PeriodEnd}");

            await _mediator.SendAsync(new CreateTransferTransactionsCommand
            {
                AccountId = message.AccountId,
                PeriodEnd = message.PeriodEnd
            });
        }
    }
}
