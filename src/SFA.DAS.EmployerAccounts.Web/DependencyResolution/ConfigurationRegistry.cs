using SFA.DAS.EmployerAccounts.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Web.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            For<EmployerAccountsConfiguration>().Use(() =>
                ConfigurationHelper.GetConfiguration<EmployerAccountsConfiguration>(
                    "SFA.DAS.EmployerAccounts")).Singleton();
        }
    }
}