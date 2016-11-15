using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Events;
using SFA.DAS.EAS.Application.Events.ProcessPayment;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.Payments.Events.Api.Client;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData
{
    public class RefreshPaymentDataCommandHandler : AsyncRequestHandler<RefreshPaymentDataCommand>
    {
        private readonly IValidator<RefreshPaymentDataCommand> _validator;
        private readonly IPaymentsEventsApiClient _paymentsEventsApiClient;
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public RefreshPaymentDataCommandHandler(IValidator<RefreshPaymentDataCommand> validator, IPaymentsEventsApiClient paymentsEventsApiClient, IDasLevyRepository dasLevyRepository, IMediator mediator, ILogger logger)
        {
            _validator = validator;
            _paymentsEventsApiClient = paymentsEventsApiClient;
            _dasLevyRepository = dasLevyRepository;
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
            PageOfResults<Payment> payments = null;
            try
            {
                payments = await _paymentsEventsApiClient.GetPayments(message.PeriodEnd, message.AccountId.ToString());
            }
            catch (WebException ex)
            {
                _logger.Error(ex,$"Unable to get payment information for {message.PeriodEnd} accountid {message.AccountId}");
            }

            if (payments == null)
            {
                return;
            }

            var sendPaymentDataChanged = false;

            foreach (var payment in payments.Items)
            {
                var existingPayment = await _dasLevyRepository.GetPaymentData(Guid.Parse(payment.Id));

                if (existingPayment != null)
                {
                    continue;
                }

                await _dasLevyRepository.CreatePaymentData(payment,message.AccountId,message.PeriodEnd);
                sendPaymentDataChanged = true;
            }

            if (sendPaymentDataChanged)
            {
                await _mediator.PublishAsync(new ProcessPaymentEvent());
            }
        }
    }
}