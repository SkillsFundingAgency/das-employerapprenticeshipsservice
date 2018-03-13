using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.NLog.Logger;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers
{
    public class RefreshAccountTransfersCommandHandler : AsyncRequestHandler<RefreshAccountTransfersCommand>
    {
        private readonly IValidator<RefreshAccountTransfersCommand> _validator;
        private readonly IPaymentService _paymentService;
        private readonly ITransferRepository _transferRepository;
        private readonly ILog _logger;


        public RefreshAccountTransfersCommandHandler(
            IValidator<RefreshAccountTransfersCommand> validator,
            IPaymentService paymentService,
            ITransferRepository transferRepository,
            ILog logger)
        {

            _validator = validator;
            _paymentService = paymentService;
            _transferRepository = transferRepository;
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
                var transfers = await _paymentService.GetAccountTransfers(message.PeriodEnd, message.AccountId);

                await _transferRepository.CreateAccountTransfers(transfers);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Could not process transfers for Account Id {message.AccountId} and Period End {message.PeriodEnd}");
                throw;
            }
        }
    }
}