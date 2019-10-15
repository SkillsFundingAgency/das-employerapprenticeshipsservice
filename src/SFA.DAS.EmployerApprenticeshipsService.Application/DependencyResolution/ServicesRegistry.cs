using SFA.DAS.EAS.Infrastructure.Factories;
using SFA.DAS.EAS.Infrastructure.Interfaces.REST;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class ServicesRegistry : Registry
    {
        public ServicesRegistry()
        {
            For<IRestClientFactory>().Use<RestClientFactory>();
            For<IRestServiceFactory>().Use<RestServiceFactory>();
        }
    }
}