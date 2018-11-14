using SFA.DAS.Configuration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            For<EmployerAccountsConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<EmployerAccountsConfiguration>("SFA.DAS.EmployerAccounts")).Singleton();
            For<EmployerFinanceConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<EmployerFinanceConfiguration>("SFA.DAS.EmployerFinance")).Singleton();
            //For<EmployerAccountsReadStoreConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<EmployerAccountsReadStoreConfiguration>("SFA.DAS.EmployerAccounts.ReadStore")).Singleton();
            For<EmployerAccountsReadStoreConfiguration>().Use(() => new EmployerAccountsReadStoreConfiguration
                {
                    Uri = "https://localhost:8081",
                    AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
            }
                ).Singleton();
        }
    }
}