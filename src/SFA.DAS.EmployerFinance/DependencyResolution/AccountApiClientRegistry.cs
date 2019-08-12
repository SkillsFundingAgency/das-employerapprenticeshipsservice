using SFA.DAS.EAS.Account.Api.Client;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class AccountApiClientRegistry : Registry
    {
        public AccountApiClientRegistry()
        {
            For<IAccountApiClient>().Use<AccountApiClient>();
            // config for the client is registered in ConfigurationRegistry
        }
    }
}
