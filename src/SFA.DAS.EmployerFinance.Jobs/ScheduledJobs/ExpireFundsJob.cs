using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.Jobs.ScheduledJobs
{
    public class ExpireFundsJob
    {
        private readonly IMessageSession _messageSession;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IEmployerAccountRepository _accountRepository;

        public ExpireFundsJob(
            IMessageSession messageSession, 
            ICurrentDateTime currentDateTime, 
            IEmployerAccountRepository accountRepository)
        {
            _messageSession = messageSession;
            _currentDateTime = currentDateTime;
            _accountRepository = accountRepository;
        }

        public async Task Run(
            [TimerTrigger("0 0 0 28 * *")] TimerInfo timer, 
            ILogger logger)
        {
            logger.LogInformation($"Starting {nameof(ExpireFundsJob)}");

            var now = _currentDateTime.Now;
            var accounts = await _accountRepository.GetAllAccounts();
            var commands = accounts.Select(a => new ExpireAccountFundsCommand { AccountId = a.Id });

            var tasks = commands.Select(c =>
            {
                var sendOptions = new SendOptions();

                sendOptions.RequireImmediateDispatch();
                sendOptions.SetMessageId($"{nameof(ExpireAccountFundsCommand)}-{now.Year}-{now.Month}-{c.AccountId}");

                return _messageSession.Send(c, sendOptions);
            });

            await Task.WhenAll(tasks);

            logger.LogInformation($"{nameof(ExpireFundsJob)} completed.");
        }
    }
}