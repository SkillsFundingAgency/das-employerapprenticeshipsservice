using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using StructureMap;

namespace SFA.DAS.EAS.Account.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesAndExecutablesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
                s.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(c => c.Singleton());
            });

            For<IWebJobConfiguration>().Use(new WebJobConfig
            {
                DashboardConnectionString = CloudConfigurationManager.GetSetting("DashboardConnectionString"),
                StorageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString")
            });

            For<JobHost>().Use(ctx => ctx.GetInstance<IJobHostFactory>().CreateJobHost());
        }
    }
}