using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.Jobs.ScheduledJobs
{
    public class ExpireFundsJob
    {
        private readonly IMessageSession _messageSession;
        private readonly ILogger _logger;

        public ExpireFundsJob(IMessageSession messageSession, ILogger logger)
        {
            _messageSession = messageSession;
            _logger = logger;
        }

        public Task Run([TimerTrigger("0 0 0 28 * *")] TimerInfo timer)
        {
            _logger.LogInformation($"ExpireFundsJob triggered");
            return _messageSession.Send(new ExpireFundsCommand());
        }
    }
}