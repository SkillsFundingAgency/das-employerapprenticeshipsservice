using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Caching;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class CachesRegistry : Registry
    {
        public CachesRegistry()
        {
            For<ICache>().Use<InMemoryCache>();
        }
    }
}