using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class EmployerAccountsApiServiceRegistry : Registry
    {
        public EmployerAccountsApiServiceRegistry()
        {
            For<IEmployerAccountsApiService>().Use<EmployerAccountsApiService>().Singleton();
            For<IEmployerAccountsApiHttpClientFactory>().Use<EmployerAccountsApiHttpClientFactory>();
        }
    }
}
