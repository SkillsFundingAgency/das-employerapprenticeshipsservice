using SFA.DAS.AutoConfiguration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.EAS.Account.Api.Client;
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
            For<EmployerAccountsConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsConfiguration>(ConfigurationKeys.EmployerAccounts)).Singleton();
            For<EmployerFinanceConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerFinanceConfiguration>(ConfigurationKeys.EmployerFinance)).Singleton();
            For<EmployerAccountsReadStoreConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsReadStoreConfiguration>(ConfigurationKeys.EmployerAccountsReadStore)).Singleton();
            For<ReferenceDataApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<ReferenceDataApiClientConfiguration>(ConfigurationKeys.ReferenceDataApi)).Singleton();
            //For<IAccountApiConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<AccountApiConfiguration>(ConfigurationKeys.ReferenceDataApi)).Singleton();
            For<IAccountApiConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().AccountApi).Singleton();
        }
    }
}