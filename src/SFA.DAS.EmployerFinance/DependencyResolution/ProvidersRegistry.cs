using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.Providers.Api.Client;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class ProvidersRegistry : Registry
    {
        public ProvidersRegistry()
        {
            For<IProviderApiClient>().Use(c => new ProviderApiClient(c.GetInstance<EmployerFinanceConfiguration>().ApprenticeshipInfoService.BaseUrl));
            For<IProviderService>().Use<ProviderServiceFromDb>();
            For<IProviderService>().DecorateAllWith<ProviderServiceRemote>();
            For<IProviderService>().DecorateAllWith<ProviderServiceCache>();
        }
    }
}