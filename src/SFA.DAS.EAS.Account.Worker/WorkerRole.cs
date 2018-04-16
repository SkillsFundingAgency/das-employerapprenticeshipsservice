using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.ServiceRuntime;
using SFA.DAS.EAS.Account.Worker.DependencyResolution;
using SFA.DAS.EAS.Account.Worker.Infrastructure;
using SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces;

namespace SFA.DAS.EAS.Account.Worker
{
    public class WorkerRole : RoleEntryPoint
    {

        public override void Run()
        {
            var container = IoC.Initialize();
            ServiceLocator.Initialise(container);

            var webjobhelper = container.GetInstance<IAzureWebJobHelper>();
            webjobhelper.EnsureAllQueuesForTriggeredJobs();

            var host = container.GetInstance<JobHost>();
            host.RunAndBlock();
        }
    }
}
