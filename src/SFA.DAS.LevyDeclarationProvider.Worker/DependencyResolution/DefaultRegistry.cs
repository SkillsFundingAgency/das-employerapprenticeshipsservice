using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.Messaging;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace SFA.DAS.LevyDeclarationProvider.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {

        public DefaultRegistry()
        {
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.EmployerApprenticeshipsService")
                    && !a.GetName().Name.Equals("SFA.DAS.EmployerApprenticeshipsService.Infrastructure"));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            //TODO add config service and use Azure service bus queue instead
            For<IPollingMessageReceiver>().Use(() => new Messaging.FileSystem.FileSystemMessageService(@".\Queue"));
            For<ILevyDeclaration>().Use<LevyDeclaration>();

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
