using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Events;
using SFA.DAS.EAS.Application.Events.ProcessPayment;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.Payments.Events.Api.Client;

namespace SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData
{
    public class RefreshPaymentDataCommandHandler : AsyncRequestHandler<RefreshPaymentDataCommand>
    {
        private readonly IValidator<RefreshPaymentDataCommand> _validator;
        private readonly IPaymentsEventsApiClient _paymentsEventsApiClient;
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IMediator _mediator;

        public RefreshPaymentDataCommandHandler(IValidator<RefreshPaymentDataCommand> validator, IPaymentsEventsApiClient paymentsEventsApiClient, IDasLevyRepository dasLevyRepository, IMediator mediator)
        {
            _validator = validator;
            _paymentsEventsApiClient = paymentsEventsApiClient;
            _dasLevyRepository = dasLevyRepository;
            _mediator = mediator;
        }

        protected override async Task HandleCore(RefreshPaymentDataCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var payments = await _paymentsEventsApiClient.GetPayments(message.PeriodEnd, message.AccountId.ToString());
            var sendPaymentDataChanged = false;

            foreach (var payment in payments.Items)
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