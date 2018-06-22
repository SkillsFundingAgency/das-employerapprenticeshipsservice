using MediatR;
using NServiceBus;
using NServiceBus.Logging;
using SFA.DAS.EAS.Application.Commands.CreateNewPeriodEnd;
using SFA.DAS.EAS.Application.Queries.GetAllEmployerAccounts;
using SFA.DAS.EAS.Application.Queries.Payments.GetCurrentPeriodEnd;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Messages.Commands;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.MessageHandlers.CommandHandlers
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

            var response = await _mediator.SendAsync(new GetAllEmployerAccountsRequest());

            foreach (var paymentsPeriodEnd in periodsToProcess)
            {
                var periodEnd = new Domain.Models.Payments.PeriodEnd
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

                var tasks = new List<Task>();

                foreach (var account in response.Accounts)
                {
                    _logger.Info($"Creating payment queue message for account ID: '{account.Id}' period end ref: '{periodEnd.PeriodEndId}'");

                    tasks.Add(context.SendLocal<ImportAccountPaymentsCommand>(c =>
                    {
                        c.AccountId = account.Id;
                        c.PeriodEndRef = periodEnd.PeriodEndId;
                    }));
                }

                await Task.WhenAll(tasks);
            }
        }
    }
}
