using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Transfers;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                _logger.Info($"Getting account transfers from payment api for AccountId = '{message.ReceiverAccountId}' and PeriodEnd = '{message.PeriodEnd}' CorrelationId: {message.CorrelationId}");

                var paymentTransfers = await _paymentService.GetAccountTransfers(message.PeriodEnd, message.ReceiverAccountId, message.CorrelationId);

                _logger.Info($"Retrieved payment transfers from payment api for AccountId = '{message.ReceiverAccountId}' and PeriodEnd = '{message.PeriodEnd}' CorrelationId: {message.CorrelationId}");

                //Handle multiple transfers for the same account, period end and commitment ID by grouping them together
                //This can happen if delivery months are different by collection months are not for payments
                var transfers = paymentTransfers.GroupBy(t => new { t.SenderAccountId, t.ReceiverAccountId, CommitmentId = t.ApprenticeshipId, t.PeriodEnd })
                    .Select(g =>
                    {
                        var firstGroupItem = g.First();

                        return new AccountTransfer
                        {
                            PeriodEnd = firstGroupItem.PeriodEnd,
                            Amount = g.Sum(x => x.Amount),
                            ApprenticeshipId = firstGroupItem.ApprenticeshipId,
                            ReceiverAccountId = firstGroupItem.ReceiverAccountId,
                            SenderAccountId = firstGroupItem.SenderAccountId,
                            Type = firstGroupItem.Type
                        };
                    }).ToArray();

                _logger.Info($"Retrieved {transfers.Length} grouped account transferts from payment api for AccountId = '{message.ReceiverAccountId}' and PeriodEnd = '{message.PeriodEnd}' CorrelationId: {message.CorrelationId}");

                await _transferRepository.CreateAccountTransfers(transfers);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Could not process transfers for Account Id {message.ReceiverAccountId} and Period End {message.PeriodEnd}, CorrelationId = {message.CorrelationId}");
                throw;
            }

            _logger.Info($"Refresh account transfers handler complete for AccountId = '{message.ReceiverAccountId}' and PeriodEnd = '{message.PeriodEnd}' CorrelationId: {message.CorrelationId}");
        }
    }
}