using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.ServiceRuntime;
using SFA.DAS.EAS.Account.Worker.DependencyResolution;
using SFA.DAS.EAS.Account.Worker.Infrastructure;
using SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces;
using StructureMap;

namespace SFA.DAS.EAS.Account.Worker
{
    public class WorkerRole : RoleEntryPoint
    {

        private IContainer _container;

        public override void Run()
        {
            _container = IoC.Initialize();
            ServiceLocator.Initialise(_container);

            var webjobhelper = _container.GetInstance<IAzureWebJobHelper>();
            webjobhelper.EnsureAllQueuesForTriggeredJobs();

            var host = _container.GetInstance<JobHost>();
            host.RunAndBlock();
        }

        public override void OnStop()
        {
            _container?.Dispose();
        }
    }
}
