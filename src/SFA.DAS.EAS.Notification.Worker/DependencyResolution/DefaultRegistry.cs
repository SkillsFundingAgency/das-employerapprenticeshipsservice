using MediatR;
using StructureMap;
using StructureMap.Graph;

namespace SFA.DAS.EAS.Notification.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            //var environment = Environment.GetEnvironmentVariable("DASENV");
            //if (string.IsNullOrEmpty(environment))
            //{
            //    environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            //}

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.EmployerApprenticeshipsService") ||
                   a.GetName().Name.StartsWith("SFA.DAS.EAS"));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });
            
            AddMediatrRegistrations();
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            For<IMediator>().Use<Mediator>();
        }
    }
}
