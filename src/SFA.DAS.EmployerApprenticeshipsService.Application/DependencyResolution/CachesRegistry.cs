using System.Configuration;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.Caches;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class CachesRegistry : Registry
    {
        public CachesRegistry()
        {
            For<IDistributedCache>().Use(c => c.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL)
                ? new LocalDevCache() as IDistributedCache
                : new RedisCache(ConfigurationManager.AppSettings["RedisConnection"]) as
                    IDistributedCache).Singleton();
        }
    }
}