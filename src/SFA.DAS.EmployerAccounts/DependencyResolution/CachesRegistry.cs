using SFA.DAS.AutoConfiguration;
using SFA.DAS.Caches;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class CachesRegistry : Registry
{
    public CachesRegistry()
    {
        For<IInProcessCache>().Use<InProcessCache>().Singleton();

        For<IDistributedCache>().Use(c => c.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL)
            ? new LocalDevCache() as IDistributedCache
            : new RedisCache(c.GetInstance<EmployerAccountsConfiguration>().RedisConnectionString) as
                IDistributedCache).Singleton();
    }
}