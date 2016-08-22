using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using StructureMap;
using StructureMap.Graph;

namespace SFA.DAS.EAS.Notification.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.EmployerApprenticeshipsService") ||
                   a.GetName().Name.StartsWith("SFA.DAS.EAS"));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();

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
