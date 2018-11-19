using Moq;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.ApiClient.TestHarness.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            For<EmployerAccountsReadStoreConfiguration>().Use(() => new EmployerAccountsReadStoreConfiguration
            {
                Uri = "https://localhost:8081",
                AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
            });

            For<IEmployerAccountsApiClientConfiguration>().Use(new EmployerAccountsApiClientConfiguration());
        }
    }
}
