using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
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

                var senderTransfers = accountTransfers.GroupBy(t => t.SenderAccountId);

                var transactions = senderTransfers.SelectMany(CreateTransactions).ToArray();

                await _transactionRepository.CreateTransferTransactions(transactions);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to create transfer transaction for accountId {message.AccountId} and period end {message.PeriodEnd}");
                throw;
            }
        }

        private static IEnumerable<TransferTransactionLine> CreateTransactions(IGrouping<long, AccountTransfer> senderTransferGroup)
        {
            if (!senderTransferGroup.Any())
                return new TransferTransactionLine[0];

            var firstTransfer = senderTransferGroup.First();

            var senderAccountId = senderTransferGroup.Key;
            var senderAccountName = firstTransfer.SenderAccountName;
            var receiverAccountId = firstTransfer.ReceiverAccountId;  //use key as we have grouped by receiver ID
            var receiverAccountName = firstTransfer.ReceiverAccountName;
            var transferTotal = senderTransferGroup.Sum(gt => gt.Amount);

            var senderTranferTransaction = new TransferTransactionLine
            {
                AccountId = senderAccountId,
                Amount = -transferTotal,        //Negative value as we are removing money from sender
                DateCreated = DateTime.Now,
                TransactionDate = DateTime.Now,
                SenderAccountId = senderAccountId,
                SenderAccountName = senderAccountName,
                ReceiverAccountId = receiverAccountId,
                ReceiverAccountName = receiverAccountName
            };

            var receiverTransferTransaction = new TransferTransactionLine
            {
                AccountId = receiverAccountId,
                Amount = transferTotal,         //Positive value as we are adding money to receiver
                DateCreated = DateTime.Now,
                TransactionDate = DateTime.Now,
                SenderAccountId = senderAccountId,
                SenderAccountName = senderAccountName,
                ReceiverAccountId = receiverAccountId,
                ReceiverAccountName = receiverAccountName
            };

            return new[] { senderTranferTransaction, receiverTransferTransaction };
        }
    }
}