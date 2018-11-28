using SFA.DAS.EmployerAccounts.ReadStore.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });

            For<EmployerAccountsReadStoreConfiguration>().Use(() => new EmployerAccountsReadStoreConfiguration
            {
                Uri = "https://localhost:8081",
                AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
            });
        }
    }
}