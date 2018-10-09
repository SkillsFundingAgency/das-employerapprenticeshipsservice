using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers
{
    public class RefreshAccountTransfersCommandHandler : AsyncRequestHandler<RefreshAccountTransfersCommand>
    {
        private readonly IValidator<RefreshAccountTransfersCommand> _validator;
        private readonly IPaymentService _paymentService;
        private readonly ITransferRepository _transferRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILog _logger;

        public RefreshAccountTransfersCommandHandler(
            IValidator<RefreshAccountTransfersCommand> validator,
            IPaymentService paymentService,
            ITransferRepository transferRepository,
            IAccountRepository accountRepository,
            IMessagePublisher messagePublisher,
            ILog logger)
        {
            _validator = validator;
            _paymentService = paymentService;
            _transferRepository = transferRepository;
            _accountRepository = accountRepository;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        protected override async Task HandleCore(RefreshAccountTransfersCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            try
            {
                var paymentTransfers = await _paymentService.GetAccountTransfers(message.PeriodEnd, message.ReceiverAccountId);

                var transfers = paymentTransfers.GroupBy(t => new { t.SenderAccountId, t.ReceiverAccountId, t.CommitmentId, t.PeriodEnd })
                    .Select(g =>
                    {
                        var firstGroupItem = g.First();
                        return new AccountTransfer
                        {
                            PeriodEnd = firstGroupItem.PeriodEnd,
                            Amount = g.Sum(x => x.Amount),
                            CommitmentId = firstGroupItem.CommitmentId,
                            ReceiverAccountId = firstGroupItem.ReceiverAccountId,
                            ReceiverAccountName = firstGroupItem.ReceiverAccountName,
                            SenderAccountId = firstGroupItem.SenderAccountId,
                            SenderAccountName = firstGroupItem.SenderAccountName,
                            Type = firstGroupItem.Type
                        };
                    }).ToArray();

                var transferSenderIds = transfers.Select(t => t.SenderAccountId).Distinct();

                var transferSenderAccountNames = await _accountRepository.GetAccountNames(transferSenderIds);

                var transferReceiverAccountName = await _accountRepository.GetAccountName(message.ReceiverAccountId);

                foreach (var transfer in transfers)
                {
                    transfer.PeriodEnd = message.PeriodEnd;

                    var paymentDetails = await _transferRepository.GetTransferPaymentDetails(transfer);

                    transfer.CourseName = paymentDetails.CourseName ?? "Unknown Course";
                    transfer.CourseLevel = paymentDetails.CourseLevel;
                    transfer.ApprenticeCount = paymentDetails.ApprenticeCount;

                    transfer.SenderAccountName = transferSenderAccountNames[transfer.SenderAccountId];
                    transfer.ReceiverAccountName = transferReceiverAccountName;

                    if (transfer.Amount != paymentDetails.PaymentTotal)
                        _logger.Warn("Transfer total does not match transfer payments total");
                }

                await _transferRepository.CreateAccountTransfers(transfers);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Could not process transfers for Account Id {message.ReceiverAccountId} and Period End {message.PeriodEnd}");
                throw;
            }
        }
    }
}