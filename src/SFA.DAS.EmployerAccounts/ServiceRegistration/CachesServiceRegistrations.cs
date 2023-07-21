using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.Caches;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class CachesServiceRegistrations
{
    public static IServiceCollection AddCachesRegistrations(this IServiceCollection services)
    {
        services.AddSingleton<ICacheStorageService, CacheStorageService>();
        services.AddSingleton<IInProcessCache, InProcessCache>();

        services.AddSingleton(s =>
        {
            var environment = s.GetService<IEnvironmentService>();
            var config = s.GetService<EmployerAccountsConfiguration>();

            return environment.IsCurrent(DasEnv.LOCAL)
                ? new LocalDevCache() as IDistributedCache
                : new RedisCache(config.RedisConnectionString);
        });

        return services;
    }
}