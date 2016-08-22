using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services;
using StructureMap;
using StructureMap.Graph;

namespace SFA.DAS.LevyDeclarationProvider.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.EmployerApprenticeshipsService") ||
                   a.GetName().Name.StartsWith("SFA.DAS.LevyDeclarationProvider"));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<IUserRepository>().Use<UserRepository>();

            For<IConfiguration>().Use<LevyDeclarationProviderConfiguration>();

            For<ILevyDeclarationService>().Use<LevyDeclarationFileBasedService>();
            
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
