using SFA.DAS.EmployerFinance.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Web.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<DateTimeRegistry>();
                c.AddRegistry<DataRegistry>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<RepositoriesRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}