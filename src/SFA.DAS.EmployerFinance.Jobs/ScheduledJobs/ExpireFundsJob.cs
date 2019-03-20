using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Messages.Commands;

namespace SFA.DAS.EmployerFinance.Jobs.ScheduledJobs
{
    public class ExpireFundsJob
    {
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IMessageSession _messageSession;

        public ExpireFundsJob(ICurrentDateTime currentDateTime, IMessageSession messageSession)
        {
            _currentDateTime = currentDateTime;
            _messageSession = messageSession;
        }

        public Task Run([TimerTrigger("0 0 0 28 * *")] TimerInfo timer, ILogger logger)
        {
            var now = _currentDateTime.Now;
            var command = new ExpireFundsCommand { Year = now.Year, Month = now.Month };
            var task = _messageSession.Send(command);

            return task;
        }
    }
}