using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Queries.GetAllEmployerAccounts;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ProcessPeriodEndPaymentsCommandHandler : IHandleMessages<ProcessPeriodEndPaymentsCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public ProcessPeriodEndPaymentsCommandHandler(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(ProcessPeriodEndPaymentsCommand message, IMessageHandlerContext context)
        {
            _logger.Info($"Creating payment queue message for all accounts for period end ref: '{message.PeriodEndRef}'");
            var response = await _mediator.SendAsync(new GetAllEmployerAccountsRequest());
            var batchSize = 5000;

            var batchedAccounts = response.Accounts
                .Select((item, inx) => new { item, inx })
                .GroupBy(x => x.inx / batchSize)
                .Select(g => g.Select(x => x.item));

            foreach (var batch in batchedAccounts)
            {
                var tasks = batch.Select(account => Task.Run(async () =>
                {
                    _logger.Info($"Creating payment queue message for account ID: '{account.Id}' period end ref: '{message.PeriodEndRef}'");

                    var sendOptions = new SendOptions();

                    sendOptions.RouteToThisEndpoint();
                    sendOptions.RequireImmediateDispatch(); // Circumvent sender outbox
                    sendOptions.SetMessageId($"{nameof(ImportAccountPaymentsCommand)}-{message.PeriodEndRef}-{account.Id}"); // Allow receiver outbox to de-dupe

                    await context.Send(new ImportAccountPaymentsCommand { PeriodEndRef = message.PeriodEndRef, AccountId = account.Id }, sendOptions).ConfigureAwait(false);
                }));

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            _logger.Info($"Completed payment message queuing for period end ref: '{message.PeriodEndRef}'");
        }
    }
}
