using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.EmployerAccounts.Jobs.DependencyResolution;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerAccounts.Jobs
{
    public static class Jobs
    {
        public static Task ProcessOutboxMessages([TimerTrigger("0 */10 * * * *")] TimerInfo timer, TraceWriter logger)
        {
            var job = ServiceLocator.GetInstance<IProcessOutboxMessagesJob>();
            return job.RunAsync();
        }
    }
}