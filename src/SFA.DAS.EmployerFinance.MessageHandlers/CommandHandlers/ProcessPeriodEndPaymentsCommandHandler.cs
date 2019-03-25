using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Queries.GetAllEmployerAccounts;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers
{
    public class ProcessPeriodPaymentsEndCommandHandler : IHandleMessages<ProcessPeriodEndPaymentsCommand>
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public ProcessPeriodPaymentsEndCommandHandler(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(ProcessPeriodEndPaymentsCommand message, IMessageHandlerContext context)
        {
            var response = await _mediator.SendAsync(new GetAllEmployerAccountsRequest());

            var tasks = new List<Task>();

            foreach (var account in response.Accounts)
            {
                _logger.Info($"Creating payment queue message for account ID: '{account.Id}' period end ref: '{message.PeriodEndRef}'");

                var sendOptions = new SendOptions();

                sendOptions.RouteToThisEndpoint();
                sendOptions.RequireImmediateDispatch(); // Circumvent sender outbox
                sendOptions.SetMessageId($"{message.PeriodEndRef}-{account.Id}"); // Allow receiver outbox to de-dupe

                tasks.Add(context.Send(new ImportAccountPaymentsCommand { PeriodEndRef = message.PeriodEndRef, AccountId = account.Id }, sendOptions));
            }

            await Task.WhenAll(tasks);
        }
    }
}
