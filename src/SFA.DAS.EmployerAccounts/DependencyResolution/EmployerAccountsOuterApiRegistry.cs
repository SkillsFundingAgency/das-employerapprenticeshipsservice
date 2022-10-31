using System.Net.Http;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;
using SFA.DAS.EmployerAccounts.Infrastructure;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class EmployerAccountsOuterApiRegistry : Registry
    {
        public EmployerAccountsOuterApiRegistry()
        {
            For<EmployerAccountsOuterApiConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().EmployerAccountsOuterApiConfiguration).Singleton();
            For<IOuterApiClient>().Use<OuterApiClient>().Ctor<HttpClient>().Is(new HttpClient()).Singleton();
        }
    }
}
