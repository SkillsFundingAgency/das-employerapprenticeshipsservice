using SFA.DAS.EmployerAccounts.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Jobs.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<DataRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}