﻿using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerFinance.Commands.CreateTransferTransactions;
using SFA.DAS.EmployerFinance.Commands.RefreshAccountTransfers;
using SFA.DAS.EmployerFinance.Commands.RefreshPaymentData;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ImportAccountPaymentsCommandHandler : IHandleMessages<ImportAccountPaymentsCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public ImportAccountPaymentsCommandHandler(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(ImportAccountPaymentsCommand message, IMessageHandlerContext context)
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
