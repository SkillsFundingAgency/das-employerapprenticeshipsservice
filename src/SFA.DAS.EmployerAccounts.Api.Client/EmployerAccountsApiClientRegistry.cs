using SFA.DAS.EmployerAccounts.Interfaces;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public class EmployerAccountsApiClientRegistry : Registry
    {
        public EmployerAccountsApiClientRegistry()
        {
            For<EmployerAccountsApiClientConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<EmployerAccountsApiClientConfiguration>("SFA.DAS.EmployerAccounts.Api.Client")).Singleton();
            For<IEmployerAccountsApiClient>().Use<EmployerAccountsApiClient>();
        }
    }
}