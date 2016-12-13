using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Events.ProcessPayment;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData
{
    public class RefreshPaymentDataCommandHandler : AsyncRequestHandler<RefreshPaymentDataCommand>
    {
        private readonly IValidator<RefreshPaymentDataCommand> _validator;

        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IPaymentService _paymentService;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public RefreshPaymentDataCommandHandler(
            IValidator<RefreshPaymentDataCommand> validator,
            IDasLevyRepository dasLevyRepository,
            IPaymentService paymentService,
            IMediator mediator,
            ILogger logger)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            _paymentService = paymentService;
            _mediator = mediator;
            _logger = logger;
        }

        protected override async Task HandleCore(RefreshPaymentDataCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }
            
            var paymentDetails = await _paymentService.GetAccountPayments(message.PeriodEnd, message.AccountId.ToString());

            if (!paymentDetails.Any())
            {
                return;
            }

            var sendPaymentDataChanged = false;

            foreach (var payment in paymentDetails)
            {
                var existingPayment = await _dasLevyRepository.GetPaymentData(Guid.Parse(payment.Id));

                if (existingPayment != null)
                {
                    continue;
                }
                
                await _dasLevyRepository.CreatePaymentData(payment);
                sendPaymentDataChanged = true;
            }

            if (sendPaymentDataChanged)
            {
                await _mediator.PublishAsync(new ProcessPaymentEvent());
            }
        }
    }
}