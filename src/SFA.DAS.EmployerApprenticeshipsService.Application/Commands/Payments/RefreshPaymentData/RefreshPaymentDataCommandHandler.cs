using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Events.ProcessPayment;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
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
        private readonly IApprenticeshipInfoServiceWrapper _apprenticeshipInfoService;

        public RefreshPaymentDataCommandHandler(
            IValidator<RefreshPaymentDataCommand> validator, 
            IPaymentsEventsApiClient paymentsEventsApiClient, 
            IDasLevyRepository dasLevyRepository, 
            IMediator mediator, 
            ILogger logger, 
            IApprenticeshipInfoServiceWrapper apprenticeshipInfoService)
        {
            _validator = validator;
            _paymentsEventsApiClient = paymentsEventsApiClient;
            _dasLevyRepository = dasLevyRepository;
            _mediator = mediator;
            _logger = logger;
            _apprenticeshipInfoService = apprenticeshipInfoService;
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

                var providerId = Convert.ToInt32(payment.Ukprn);
                var providerView = _apprenticeshipInfoService.GetProvider(providerId);
                var providerName = providerView?.Providers?.FirstOrDefault()?.ProviderName;

                string courseName;

                if (payment.StandardCode.HasValue)
                {
                    var standardsView = await _apprenticeshipInfoService.GetStandardsAsync();

                    courseName = standardsView.Standards.SingleOrDefault(s =>
                            s.Code.Equals(payment.StandardCode.Value))?.Title ?? string.Empty;
                }
                else if (payment.FrameworkCode.HasValue && payment.ProgrammeType.HasValue && payment.PathwayCode.HasValue)
                {
                    var frameworksView = await _apprenticeshipInfoService.GetFrameworksAsync();

                    courseName = frameworksView.Frameworks.SingleOrDefault(f =>
                                     f.FrameworkCode.Equals(payment.FrameworkCode.Value) &&
                                     f.ProgrammeType.Equals(payment.ProgrammeType.Value) &&
                                     f.PathwayCode.Equals(payment.PathwayCode.Value))?.Title ?? string.Empty;
                }
                else
                {
                    courseName = string.Empty;
                }

                await _dasLevyRepository.CreatePaymentData(payment,message.AccountId,message.PeriodEnd, providerName, courseName);
                sendPaymentDataChanged = true;
            }

            if (sendPaymentDataChanged)
            {
                await _mediator.PublishAsync(new ProcessPaymentEvent());
            }
        }
    }
}