using SFA.DAS.AutoConfiguration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            IncludeRegistry<AutoConfigurationRegistry>();
            For<EmployerAccountsConfiguration>().Use(c => c.GetInstance<ITableStorageConfigurationService>().Get<EmployerAccountsConfiguration>("SFA.DAS.EmployerAccounts")).Singleton();
            For<EmployerFinanceConfiguration>().Use(c => c.GetInstance<ITableStorageConfigurationService>().Get<EmployerFinanceConfiguration>("SFA.DAS.EmployerFinance")).Singleton();
            For<EmployerAccountsReadStoreConfiguration>().Use(c => c.GetInstance<ITableStorageConfigurationService>().Get<EmployerAccountsReadStoreConfiguration>("SFA.DAS.EmployerAccounts.ReadStore")).Singleton();
        }
    }
}