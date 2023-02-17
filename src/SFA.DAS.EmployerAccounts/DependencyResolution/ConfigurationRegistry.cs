using SFA.DAS.Authorization.EmployerFeatures.Configuration;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces.Hmrc;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class ConfigurationRegistry : Registry
{
    public ConfigurationRegistry()
    {
        IncludeRegistry<AutoConfigurationRegistry>();
        For<EmployerAccountsConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsConfiguration>(ConfigurationKeys.EmployerAccounts)).Singleton();
        For<EmployerAccountsReadStoreConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsReadStoreConfiguration>(ConfigurationKeys.EmployerAccountsReadStore)).Singleton();
        For<ReferenceDataApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<ReferenceDataApiClientConfiguration>(ConfigurationKeys.ReferenceDataApiClient)).Singleton();
        For<IAccountApiConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().AccountApi).Singleton();
        For<EmployerFeaturesConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerFeaturesConfiguration>(ConfigurationKeys.Features)).Singleton();
        For<IHmrcConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().Hmrc).Singleton();
    }
}