using SFA.DAS.EAS.LevyAccountUpdater.WebJob.Updater;
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

            For<IAccountUpdater>().Use<AccountUpdater>();
        }
    }
}