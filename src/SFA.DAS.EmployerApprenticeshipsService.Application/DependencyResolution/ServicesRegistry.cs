using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Factories;
using SFA.DAS.EAS.Infrastructure.Interfaces.REST;
using SFA.DAS.EAS.Infrastructure.Services;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ServicesRegistry : Registry
    {
        public ServicesRegistry()
        {
            For<IAddressLookupService>().Use<AddressLookupService>();
            For<IRestClientFactory>().Use<RestClientFactory>();
            For<IRestServiceFactory>().Use<RestServiceFactory>();
        }
    }
}