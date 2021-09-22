using System.Net.Http;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Infrastructure;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Services;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class ManageApprenticeshipsOuterApiRegistry : Registry
    {
        public ManageApprenticeshipsOuterApiRegistry ()
        {
            For<ManageApprenticeshipsOuterApiConfiguration>().Use(c => c.GetInstance<EmployerFinanceConfiguration>().ManageApprenticeshipsOuterApiConfiguration).Singleton();
            For<IApiClient>().Use<ApiClient>().Ctor<HttpClient>().Is(new HttpClient()).Singleton();
        }
    }
}