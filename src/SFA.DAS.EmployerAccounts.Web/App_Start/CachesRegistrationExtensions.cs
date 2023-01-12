using SFA.DAS.AutoConfiguration;
using SFA.DAS.Caches;

namespace SFA.DAS.EmployerAccounts.Web;

public static class CachesRegistrationExtensions
{
    public static void AddCachesRegistrations(this IServiceCollection services)
    {
        services.AddSingleton<IInProcessCache, InProcessCache>();

        services.AddSingleton(s =>
        {
            var environment = s.GetService<IEnvironmentService>();
            var config = s.GetService<EmployerAccountsConfiguration>();

            return environment.IsCurrent(DasEnv.LOCAL)
                ? new LocalDevCache() as IDistributedCache
                : new RedisCache(config.RedisConnectionString);
        });
    }
}