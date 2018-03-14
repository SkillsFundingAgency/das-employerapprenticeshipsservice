using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
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
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILog _logger;


        public RefreshAccountTransfersCommandHandler(
            IValidator<RefreshAccountTransfersCommand> validator,
            IPaymentService paymentService,
            ITransferRepository transferRepository,
            IMessagePublisher messagePublisher,
            ILog logger)
        {

            _validator = validator;
            _paymentService = paymentService;
            _transferRepository = transferRepository;
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
                var paymentTransfers = await _paymentService.GetAccountTransfers(message.PeriodEnd, message.AccountId);

                var transfers = paymentTransfers.ToArray();

                await _transferRepository.CreateAccountTransfers(transfers);

                await _messagePublisher.PublishAsync(new AccountTransfersCreatedQueueMessage
                {
                    SenderAccountId = message.AccountId,
                    PeriodEnd = message.PeriodEnd
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Could not process transfers for Account Id {message.AccountId} and Period End {message.PeriodEnd}");
                throw;
            }
        }
    }
}