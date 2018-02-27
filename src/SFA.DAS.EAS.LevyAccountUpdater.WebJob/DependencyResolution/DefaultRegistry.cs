using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.LevyAccountUpdater.WebJob.Updater;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EAS.LevyAccountUpdater.WebJob.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();
            For<IAccountUpdater>().Use<AccountUpdater>();

            RegisterLogger();
        }

        private void RegisterLogger()
        {
            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                null,
                null)).AlwaysUnique();
        }
    }
}
