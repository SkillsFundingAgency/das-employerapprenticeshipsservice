using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.EmployerAccounts.Jobs.DependencyResolution;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.EmployerAccounts.Jobs.ScheduledJobs
{
    public static class Jobs
    {
        public static Task ProcessOutboxMessages([TimerTrigger("0 */10 * * * *")] TimerInfo timer, TraceWriter logger)
        {
            var job = ServiceLocator.GetInstance<IProcessClientOutboxMessagesJob>();
            return job.RunAsync();
        }
    }
}