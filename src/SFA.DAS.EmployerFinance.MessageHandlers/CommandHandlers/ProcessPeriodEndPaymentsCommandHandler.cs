using System.Collections.Generic;
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
            var response = await _mediator.SendAsync(new GetAllEmployerAccountsRequest());
            var accounts = response.Accounts;

            var messageTasks = new List<Task>();
            var sendCounter = 0;

            foreach (var account in accounts)
            {
                _logger.Info($"Creating payment queue message for account ID: '{account.Id}' period end ref: '{message.PeriodEndRef}'");

                var sendOptions = new SendOptions();

                sendOptions.RouteToThisEndpoint();
                sendOptions.SetMessageId($"{nameof(ImportAccountPaymentsCommand)}-{message.PeriodEndRef}-{account.Id}"); // Allow receiver outbox to de-dupe

                messageTasks.Add(context.Send(new ImportAccountPaymentsCommand { PeriodEndRef = message.PeriodEndRef, AccountId = account.Id }, sendOptions));
                sendCounter++;

                if (sendCounter % 1000 == 0)
                {
                    await Task.WhenAll(messageTasks);
                    _logger.Info($"Queued {sendCounter} of {accounts.Count} messages.");
                    messageTasks.Clear();
                }
            }

            // await final tasks not % 1000
            await Task.WhenAll(messageTasks);

            _logger.Info($"Completed payment message queuing for period end ref: '{message.PeriodEndRef}'");
        }
    }
}
