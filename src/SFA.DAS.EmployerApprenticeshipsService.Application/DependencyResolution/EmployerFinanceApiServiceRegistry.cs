using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class EmployerFinanceApiServiceRegistry : Registry
    {
        public EmployerFinanceApiServiceRegistry()
        {
            For<IEmployerFinanceApiService>().Use<EmployerFinanceApiService>();
            For<IEmployerFinanceApiHttpClientFactory>().Use<EmployerFinanceApiHttpClientFactory>();
        }
    }
}
