using Microsoft.Azure.WebJobs;
using SFA.DAS.EAS.DbMaintenance.WebJob.DependencyResolution;
using SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure;
using SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure.Interfaces;

namespace SFA.DAS.EAS.DbMaintenance.WebJob
{
    public class Program
    {
        public static void Main()
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