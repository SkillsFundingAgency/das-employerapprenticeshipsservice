using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Infrastructure.Caching
{
    public class CacheRegistry : Registry
    {
        public CacheRegistry()
        {
            For<IInProcessCache>().Use<InProcessCache>().Singleton();

            if (ConfigurationHelper.IsAnyOf(DasEnvironment.Local))
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
