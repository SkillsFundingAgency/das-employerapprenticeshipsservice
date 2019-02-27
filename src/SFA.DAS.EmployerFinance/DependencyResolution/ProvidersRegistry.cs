using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.Providers.Api.Client;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class ProvidersRegistry : Registry
    {
        public ProvidersRegistry()
        {
            // TODO: sort out the base uri
            For<IProviderApiClient>().Use<ProviderApiClient>().Ctor<string>("baseUri").Is(@"http://das-prd-apprenticeshipinfoservice.cloudapp.net");

            For<IProviderService>().Use<ProviderServiceFromDb>();
            For<IProviderService>().DecorateAllWith<ProviderServiceRemote>();
            For<IProviderService>().DecorateAllWith<ProviderServiceCache>();
        }
    }
}