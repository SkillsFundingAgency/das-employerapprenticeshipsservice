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
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<DataRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
