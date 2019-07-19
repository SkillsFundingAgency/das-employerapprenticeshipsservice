using System;
using Microsoft.Azure;
using SFA.DAS.Activities.Configuration;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.Caches;
using SFA.DAS.Configuration;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class CachesRegistry : Registry
    {
        public CachesRegistry()
        {
            For<IDistributedCache>().Use(c => c.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL)
                ? new LocalDevCache() as IDistributedCache
                : new RedisCache(CloudConfigurationManager.GetSetting("RedisConnection")) as
                    IDistributedCache).Singleton();
        }
    }
}