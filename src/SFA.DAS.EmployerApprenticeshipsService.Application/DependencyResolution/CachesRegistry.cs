﻿using SFA.DAS.Caches;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class CachesRegistry : Registry
    {
        public CachesRegistry()
        {
            For<IInProcessCache>().Use<InProcessCache>().Singleton();

            if (ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local))
            {
                For<IDistributedCache>().Use<LocalDevCache>().Singleton();
            }
            else
            {
                For<IDistributedCache>().Use<RedisCache>().Singleton();
            }
        }
    }
}