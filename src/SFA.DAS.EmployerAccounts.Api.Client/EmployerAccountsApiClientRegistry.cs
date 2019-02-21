using SFA.DAS.AutoConfiguration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;
using SFA.DAS.EmployerAccounts.ReadStore.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public class EmployerAccountsApiClientRegistry : Registry
    {
        public EmployerAccountsApiClientRegistry()
        {
            IncludeRegistry<AutoConfigurationRegistry>();
            For<IEmployerAccountsApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsApiClientConfiguration>("SFA.DAS.EmployerAccounts.Api.Client")).Singleton();
            For<EmployerAccountsReadStoreConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsReadStoreConfiguration>("SFA.DAS.EmployerAccounts.ReadStore")).Singleton();
            IncludeRegistry<ReadStoreDataRegistry>();
            IncludeRegistry<ReadStoreMediatorRegistry>();
            For<IEmployerAccountsApiClient>().Use<EmployerAccountsApiClient>();
        }
    }
}