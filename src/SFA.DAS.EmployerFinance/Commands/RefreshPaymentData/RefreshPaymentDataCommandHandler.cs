using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.Validation;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Events.ProcessPayment;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.EmployerFinance.Commands.RefreshPaymentData
{
    public class RefreshPaymentDataCommandHandler : AsyncRequestHandler<RefreshPaymentDataCommand>
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IValidator<RefreshPaymentDataCommand> _validator;
        private readonly IPaymentService _paymentService;
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IMediator _mediator;
        private readonly ILog _logger;


        public RefreshPaymentDataCommandHandler(
            IEventPublisher eventPublisher,
            IValidator<RefreshPaymentDataCommand> validator,
            IPaymentService paymentService,
            IDasLevyRepository dasLevyRepository,
            IMediator mediator,
            ILog logger)
        {
            _eventPublisher = eventPublisher;
            _validator = validator;
            _paymentService = paymentService;
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

            ICollection<PaymentDetails> payments = null;

            try
            {
                _logger.Info($"GetAccountPayments for AccountId = '{message.AccountId}' and PeriodEnd = '{message.PeriodEnd}'");

                payments = await _paymentService.GetAccountPayments(message.PeriodEnd, message.AccountId);
            }
            catch (WebException ex)
            {
                _logger.Error(ex, $"Unable to get payment information for AccountId = '{message.AccountId}' and PeriodEnd = '{message.PeriodEnd}'");
            }

            if (payments == null || !payments.Any())
            {
                _logger.Info($"GetAccountPayments did not find any payments for AccountId = '{message.AccountId}' and PeriodEnd = '{message.PeriodEnd}'");

                await PublishRefreshPaymentDataCompletedEvent(message, false);

                return;
            }

            _logger.Info($"GetAccountPaymentIds for AccountId = '{message.AccountId}' and PeriodEnd = '{message.PeriodEnd}'");

            var existingPaymentIds = await _dasLevyRepository.GetAccountPaymentIds(message.AccountId);
            var newPayments = payments.Where(p => !existingPaymentIds.Contains(p.Id)).ToArray();

            if (!newPayments.Any())
            {
                _logger.Info($"No new payments for AccountId = '{message.AccountId}' and PeriodEnd = '{message.PeriodEnd}'");

                await PublishRefreshPaymentDataCompletedEvent(message, false);

                return;
            }

            _logger.Info($"CreatePayments for new payments AccountId = '{message.AccountId}' and PeriodEnd = '{message.PeriodEnd}'");

            var newNonFullyFundedPayments = newPayments.Where(p => p.FundingSource != FundingSource.FullyFundedSfa);

            await _dasLevyRepository.CreatePayments(newNonFullyFundedPayments);
            await _mediator.PublishAsync(new ProcessPaymentEvent { AccountId = message.AccountId });

            await PublishRefreshPaymentDataCompletedEvent(message, true);

            _logger.Info($"Finished publishing ProcessPaymentEvent and PaymentCreatedMessage messages for AccountId = '{message.AccountId}' and PeriodEnd = '{message.PeriodEnd}'");
        }

        private async Task PublishRefreshPaymentDataCompletedEvent(RefreshPaymentDataCommand message, bool hasPayments)
        {
            await _eventPublisher.Publish(new RefreshPaymentDataCompletedEvent()
            {
                AccountId = message.AccountId,
                Created = DateTime.UtcNow,
                PeriodEnd = message.PeriodEnd,
                PaymentsProcessed = hasPayments
            });
        }
    }
}