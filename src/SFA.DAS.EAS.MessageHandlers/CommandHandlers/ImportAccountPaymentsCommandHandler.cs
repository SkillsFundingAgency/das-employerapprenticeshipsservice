using MediatR;
using NServiceBus;
using NServiceBus.Logging;
using SFA.DAS.EAS.Application.Commands.CreateTransferTransactions;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers;
using SFA.DAS.EAS.Messages.Commands;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.CommandHandlers
{
    public class ImportAccountPaymentsCommandHandler : IHandleMessages<IImportAccountPaymentsCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public ImportAccountPaymentsCommandHandler(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(IImportAccountPaymentsCommand message, IMessageHandlerContext context)
        {
            _logger.Info($"Processing refresh payment command for Account ID: {message.AccountId} PeriodEnd: {message.PeriodEndRef}");

            await _mediator.SendAsync(new RefreshPaymentDataCommand
            {
                AccountId = message.AccountId,
                PeriodEnd = message.PeriodEndRef
            });

            _logger.Info($"Processing refresh account transfers command for AccountId:{message.AccountId} PeriodEnd:{message.PeriodEndRef}");

            await _mediator.SendAsync(new RefreshAccountTransfersCommand
            {
                ReceiverAccountId = message.AccountId,
                PeriodEnd = message.PeriodEndRef
            });

            _logger.Info($"Processing create account transfer transactions command for AccountId:{message.AccountId} PeriodEnd:{message.PeriodEndRef}");

            await _mediator.SendAsync(new CreateTransferTransactionsCommand
            {
                ReceiverAccountId = message.AccountId,
                PeriodEnd = message.PeriodEndRef
            });
        }
    }
}
