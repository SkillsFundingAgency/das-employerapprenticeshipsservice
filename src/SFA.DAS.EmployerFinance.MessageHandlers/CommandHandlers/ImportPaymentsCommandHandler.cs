using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerFinance.Commands.CreateNewPeriodEnd;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Queries.GetCurrentPeriodEnd;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ImportPaymentsCommandHandler : IHandleMessages<ImportPaymentsCommand>
    {
        private readonly IPaymentsEventsApiClient _paymentsEventsApiClient;
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly PaymentsApiClientConfiguration _configuration;

        public ImportPaymentsCommandHandler(
            IPaymentsEventsApiClient paymentsEventsApiClient,
            IMediator mediator,
            ILog logger,
            PaymentsApiClientConfiguration configuration)
        {
            _paymentsEventsApiClient = paymentsEventsApiClient;
            _mediator = mediator;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Handle(ImportPaymentsCommand message, IMessageHandlerContext context)
        {
            _logger.Info($"Calling Payments API");

            if (_configuration.PaymentsDisabled)
            {
                _logger.Info("Payment processing disabled");
                return;
            }

            var periodEnds = await _paymentsEventsApiClient.GetPeriodEnds();

            var result = await _mediator.SendAsync(new GetCurrentPeriodEndRequest());//order by completion date
            var periodFound = result.CurrentPeriodEnd?.PeriodEndId == null;

            var periodsToProcess = new List<PeriodEnd>();
            if (!periodFound)
            {
                var lastPeriodId = result.CurrentPeriodEnd.PeriodEndId;

                foreach (var periodEnd in periodEnds)
                {
                    if (periodFound)
                    {
                        periodsToProcess.Add(periodEnd);
                    }
                    else if (periodEnd.Id.Equals(lastPeriodId))
                    {
                        periodFound = true;
                    }
                }
            }
            else
            {
                periodsToProcess.AddRange(periodEnds);
            }

            if (!periodsToProcess.Any())
            {
                _logger.Info("No Period Ends to Process");
                return;
            }

            foreach (var paymentsPeriodEnd in periodsToProcess)
            {
                var periodEnd = new Models.Payments.PeriodEnd
                {
                    PeriodEndId = paymentsPeriodEnd.Id,
                    CalendarPeriodMonth = paymentsPeriodEnd.CalendarPeriod?.Month ?? 0,
                    CalendarPeriodYear = paymentsPeriodEnd.CalendarPeriod?.Year ?? 0,
                    CompletionDateTime = paymentsPeriodEnd.CompletionDateTime,
                    AccountDataValidAt = paymentsPeriodEnd.ReferenceData?.AccountDataValidAt,
                    CommitmentDataValidAt = paymentsPeriodEnd.ReferenceData?.CommitmentDataValidAt,
                    PaymentsForPeriod = paymentsPeriodEnd.Links?.PaymentsForPeriod ?? string.Empty
                };

                _logger.Info($"Creating period end {periodEnd.PeriodEndId}");
                await _mediator.SendAsync(new CreateNewPeriodEndCommand { NewPeriodEnd = periodEnd });

                if (!periodEnd.AccountDataValidAt.HasValue || !periodEnd.CommitmentDataValidAt.HasValue)
                {
                    continue;
                }

                _logger.Info($"Creating process period end queue message for period end ref: '{paymentsPeriodEnd.Id}'");
                var options = new SendOptions();
                options.RouteToThisEndpoint();
                options.RequireImmediateDispatch();
                await context.Send(new ProcessPeriodEndPaymentsCommand
                {
                    PeriodEndRef = paymentsPeriodEnd.Id
                }, options);
            }
        }
    }
}
