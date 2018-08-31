using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Validation;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Commands.RefreshAccountTransfers
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

                var transfers = paymentTransfers.ToArray();

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