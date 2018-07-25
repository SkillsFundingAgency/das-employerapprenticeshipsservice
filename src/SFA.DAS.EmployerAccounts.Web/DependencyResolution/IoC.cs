using SFA.DAS.EmployerAccounts.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Web.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<DataRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
