using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using NServiceBus;
using SFA.DAS.EmployerFinance.Jobs.DependencyResolution;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.EmployerFinance.Jobs
{
    public static class Jobs
    {
        public static Task ImportLevyDeclarations([TimerTrigger("0 0 15 20 * *")] TimerInfo timer, TraceWriter logger)
        {
            var messageSession = ServiceLocator.GetInstance<IMessageSession>();
            return messageSession.Send(new ImportLevyDeclarationsCommand());
        }

        public static Task ImportPayments([TimerTrigger("0 0 * * * *")] TimerInfo timer, TraceWriter logger)
        {
            var messageSession = ServiceLocator.GetInstance<IMessageSession>();
            return messageSession.Send(new ImportPaymentsCommand());
        }

        public static Task ProcessOutboxMessages([TimerTrigger("0 */10 * * * *")] TimerInfo timer, TraceWriter logger)
        {
            var job = ServiceLocator.GetInstance<IProcessClientOutboxMessagesJob>();
            return job.RunAsync();
        }
    }
}