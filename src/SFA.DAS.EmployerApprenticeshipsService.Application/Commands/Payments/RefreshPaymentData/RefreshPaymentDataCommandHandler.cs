using MediatR;
using SFA.DAS.EAS.Application.Events.ProcessPayment;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus;
using System;
using SFA.DAS.Provider.Events.Api.Types;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData
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

                return;
            }

            _logger.Info($"GetAccountPaymentIds for AccountId = '{message.AccountId}' and PeriodEnd = '{message.PeriodEnd}'");

            var existingPaymentIds = await _dasLevyRepository.GetAccountPaymentIds(message.AccountId);
            var newPayments = payments.Where(p => !existingPaymentIds.Contains(p.Id)).ToArray();

            if (!newPayments.Any())
            {
                _logger.Info($"No new payments for AccountId = '{message.AccountId}' and PeriodEnd = '{message.PeriodEnd}'");

                return;
            }

            _logger.Info($"CreatePayments for new payments AccountId = '{message.AccountId}' and PeriodEnd = '{message.PeriodEnd}'");

            var newNonFullyFundedPayments = newPayments.Where(p => p.FundingSource != FundingSource.FullyFundedSfa);

            await _dasLevyRepository.CreatePayments(newNonFullyFundedPayments);
            await _mediator.PublishAsync(new ProcessPaymentEvent { AccountId = message.AccountId });

            var paymentEventTasks = new Task[newPayments.Length];

            for (var index = 0; index < newPayments.Length; index++)
            {
                var payment = newPayments[index];

                paymentEventTasks[index] = _eventPublisher.Publish<CreatedPaymentEvent>(e =>
                {
                    e.AccountId = payment.EmployerAccountId;
                    e.ProviderName = payment.ProviderName;
                    e.Amount = payment.Amount;
                    e.Created = DateTime.UtcNow;
                });
            }

            Task.WaitAll(paymentEventTasks);

            _logger.Info($"Finished publishing ProcessPaymentEvent and PaymentCreatedMessage messages for AccountId = '{message.AccountId}' and PeriodEnd = '{message.PeriodEnd}'");
        }
    }
}