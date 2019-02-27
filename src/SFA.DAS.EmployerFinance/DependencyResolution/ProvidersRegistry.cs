using SFA.DAS.EmployerFinance.Services;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class ProvidersRegistry : Registry
    {
        public ProvidersRegistry()
        {
            For<IProviderService>().Use<ProviderServiceFromDb>();
            For<IProviderService>().DecorateAllWith<ProviderServiceRemote>();
            For<IProviderService>().DecorateAllWith<ProviderServiceCache>();
        }
    }
}