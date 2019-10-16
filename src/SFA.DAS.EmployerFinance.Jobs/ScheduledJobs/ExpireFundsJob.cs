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

        public ExpireFundsJob(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public Task Run([TimerTrigger("0 0 0 28 * *")] TimerInfo timer, ILogger logger)
        {
            return _messageSession.Send(new ExpireFundsCommand());
        }
    }
}