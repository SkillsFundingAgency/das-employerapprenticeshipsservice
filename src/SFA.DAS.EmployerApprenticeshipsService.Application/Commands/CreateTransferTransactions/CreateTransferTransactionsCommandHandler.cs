using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.NLog.Logger;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Commands.CreateTransferTransactions
{
    public class CreateTransferTransactionsCommandHandler : AsyncRequestHandler<CreateTransferTransactionsCommand>
    {
        private readonly IValidator<CreateTransferTransactionsCommand> _validator;
        private readonly ITransferRepository _transferRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILog _logger;


        public CreateTransferTransactionsCommandHandler(
            IValidator<CreateTransferTransactionsCommand> validator,
            ITransferRepository transferRepository,
            ITransactionRepository transactionRepository,
            ILog logger)
        {
            _validator = validator;
            _transferRepository = transferRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        protected override async Task HandleCore(CreateTransferTransactionsCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            try
            {
                var transfers = await _transferRepository.GetAccountTransfersByPeriodEnd(message.AccountId, message.PeriodEnd);

                var accountTransfers = transfers as AccountTransfer[] ?? transfers.ToArray();

                var receiverTransfers = accountTransfers.GroupBy(t => t.ReceiverAccountId).ToArray();

                var transactions = receiverTransfers.Select(t =>
                {
                    var groupTransfers = t.ToArray();
                    var receiverAccountId = t.Key;
                    var receiverAccountName = groupTransfers.First().ReceiverAccountName;
                    var transferTotal = groupTransfers.Sum(gt => gt.Amount);

                    return new TransferTransactionLine
                    {
                        AccountId = groupTransfers[0].SenderAccountId,
                        Amount = -transferTotal,
                        DateCreated = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        ReceiverAccountId = receiverAccountId,
                        ReceiverAccountName = receiverAccountName
                    };
                }).ToArray();

                await _transactionRepository.CreateTransferTransactions(transactions);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to create transfer transaction for accountId {message.AccountId} and period end {message.PeriodEnd}");
                throw;
            }
        }
    }
}