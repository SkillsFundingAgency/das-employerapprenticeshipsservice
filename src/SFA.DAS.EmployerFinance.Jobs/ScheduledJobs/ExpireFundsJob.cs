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

            logger.LogInformation($"Queueing {nameof(ExpireAccountFundsCommand)} messages for {accounts.Count} accounts.");

            for (int i = 0; i < accounts.Count; i++)
            {
                var account = accounts[i];

                var sendOptions = new SendOptions();

                sendOptions.SetMessageId($"{nameof(ExpireAccountFundsCommand)}-{now.Year}-{now.Month}-{account.Id}");

                await _messageSession.Send(new ExpireAccountFundsCommand { AccountId = account.Id }, sendOptions);

                if (i % 1000 == 0)
                {
                    logger.LogInformation($"Queued {i} of {accounts.Count} messages.");
                    await Task.Delay(5000);
                }
            }

            logger.LogInformation($"{nameof(ExpireFundsJob)} completed.");
        }
    }
}