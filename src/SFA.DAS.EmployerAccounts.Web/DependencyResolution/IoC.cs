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
                c.AddRegistry<RepositoriesRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<DataRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
