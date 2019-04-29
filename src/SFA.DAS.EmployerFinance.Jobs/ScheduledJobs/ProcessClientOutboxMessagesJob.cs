using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.EmployerFinance.Jobs.ScheduledJobs
{
    public class ProcessClientOutboxMessagesJob
    {
        private readonly IProcessClientOutboxMessagesJob _processClientOutboxMessagesJob;

        public ProcessClientOutboxMessagesJob(IProcessClientOutboxMessagesJob processClientOutboxMessagesJob)
        {
            _processClientOutboxMessagesJob = processClientOutboxMessagesJob;
        }

        public Task Run([TimerTrigger("0 */10 * * * *", RunOnStartup = true)] TimerInfo timer, ILogger logger)
        {
            return _processClientOutboxMessagesJob.RunAsync();
        }
    }
}