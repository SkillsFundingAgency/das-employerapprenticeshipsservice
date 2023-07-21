using SFA.DAS.CosmosDb;

namespace SFA.DAS.EmployerAccounts.ReadStore.Configuration;

public class EmployerAccountsReadStoreConfiguration : ICosmosDbConfiguration
{
    public string Uri { get; set; }
    public string AuthKey { get; set; }
}