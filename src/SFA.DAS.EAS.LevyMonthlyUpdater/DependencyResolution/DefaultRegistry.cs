using SFA.DAS.EAS.LevyAccountUpdater.Updater;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using StructureMap;

namespace SFA.DAS.EAS.LevyAccountUpdater.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            For<IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();
            For<IAccountUpdater>().Use<AccountUpdater>();
        }
    }
}
