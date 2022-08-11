using SFA.DAS.EmployerFinance.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Jobs.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<EmployerFinanceOuterApiRegistry>();
                c.AddRegistry<DataRegistry>();
                c.AddRegistry<DateTimeRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<StartupRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}