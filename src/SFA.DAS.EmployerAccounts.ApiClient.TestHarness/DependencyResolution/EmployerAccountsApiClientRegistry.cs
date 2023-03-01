using SFA.DAS.AutoConfiguration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.ApiClient.TestHarness.DependencyResolution;

public class EmployerAccountsApiClientRegistry : Registry
{
    public EmployerAccountsApiClientRegistry()
    {
        For<EmployerAccountsReadStoreConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsReadStoreConfiguration>("SFA.DAS.EmployerAccounts.ReadStore")).Singleton();
        For<IEmployerAccountsApiClient>().Use<EmployerAccountsApiClient>();
        For<IEmployerAccountsApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsApiClientConfiguration>("SFA.DAS.EmployerAccounts.Api.Client")).Singleton();
        For<ISecureHttpClient>().Use<SecureHttpClient>();
        IncludeRegistry<AutoConfigurationRegistry>();
        IncludeRegistry<ReadStoreDataRegistry>();
    }
}