

namespace SFA.DAS.EmployerAccounts.Web.DependencyResolution
{
    using SFA.DAS.EmployerAccounts.DependencyResolution;
    using StructureMap;

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
