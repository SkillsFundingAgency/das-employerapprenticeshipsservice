using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using NServiceBus;
using SFA.DAS.EAS.Jobs.DependencyResolution;
using SFA.DAS.EAS.Messages.Commands;

namespace SFA.DAS.EAS.Jobs
{
    public class Jobs
    {
        public void ImportLevyDeclarations([TimerTrigger("0 0 15 20 * *")] TimerInfo timer, TraceWriter logger)
        {
            var endpoint = ServiceLocator.Get<IEndpointInstance>();

            endpoint.Send<IImportLevyDeclarationsCommand>(c => { });
        }

        public void ImportPayments([TimerTrigger("0 0 * * * *")] TimerInfo timer, TraceWriter logger)
        {
            var endpoint = ServiceLocator.Get<IEndpointInstance>();

            endpoint.Send<IImportPaymentsCommand>(c => { });
        }
    }
}
