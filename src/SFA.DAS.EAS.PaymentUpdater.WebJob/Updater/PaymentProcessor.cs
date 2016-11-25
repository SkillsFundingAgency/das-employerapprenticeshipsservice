using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Commands.CreateNewPeriodEnd;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetAllEmployerAccounts;
using SFA.DAS.EAS.Application.Queries.Payments.GetCurrentPeriodEnd;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.Messaging;
using SFA.DAS.Payments.Events.Api.Client;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.EAS.PaymentUpdater.WebJob.Updater
{
    public class PaymentProcessor : IPaymentProcessor
    {

        [QueueName]
        public string refresh_payments { get; set; }

        private readonly IPaymentsEventsApiClient _paymentsEventsApiClient;
        private readonly IMediator _mediator;
        private readonly IMessagePublisher _publisher;
        private readonly ILogger _logger;


        public PaymentProcessor(IPaymentsEventsApiClient paymentsEventsApiClient, IMediator mediator, IMessagePublisher publisher, ILogger logger)
        {
            _paymentsEventsApiClient = paymentsEventsApiClient;
            _mediator = mediator;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task RunUpdate()
        {
            _logger.Info($"Calling Payments API");
            var periodEnds = await _paymentsEventsApiClient.GetPeriodEnds();

            var result = await _mediator.SendAsync(new GetCurrentPeriodEndRequest());//order by completion date
            var periodFound = result.CurrentPeriodEnd?.Id == null;
            var periodsToProcess = new List<PeriodEnd>();
            if (!periodFound)
            {
                var lastPeriodId = result.CurrentPeriodEnd.Id;
                
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
            
            foreach (var periodEnd in periodsToProcess)
            {
                _logger.Info($"Creating period end {periodEnd.Id}");
                await _mediator.SendAsync(new CreateNewPeriodEndCommand {NewPeriodEnd = periodEnd});
                
                foreach (var account in response.Accounts)
                {
                    _logger.Info($"Createing payment queue message for accountId:{account.Id} periodEndId:{periodEnd.Id}");

                    await _publisher.PublishAsync(new PaymentProcessorQueueMessage
                    {
                        AccountPaymentUrl = $"{periodEnd.Links.PaymentsForPeriod}&employeraccountid={account.Id}",
                        AccountId = account.Id,
                        PeriodEndId = periodEnd.Id
                    });
                }

            }
        }
    }
}
