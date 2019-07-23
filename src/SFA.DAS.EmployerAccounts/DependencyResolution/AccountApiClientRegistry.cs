using SFA.DAS.EAS.Account.Api.Client;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class AccountApiClientRegistry : Registry
    {
        // we add this here, rather than in the client project, so we don't introduce a dependency on structuremap in the client
        public AccountApiClientRegistry()
        {
            // the client's not very good, but the version of sfa.das.http currently in EAS isn't either, so we go with the client
            For<IAccountApiClient>().Use<AccountApiClient>();
            // config is registered in ConfigurationRegistry
        }
    }
}
