using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.Api.ServiceRegistrations;

public static class CachingServiceRegistrations
{
    public static IServiceCollection AddDasDistributedMemoryCache(this IServiceCollection services, EmployerAccountsConfiguration configuration, bool isDevelopment)
    {
        if (isDevelopment)
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(o => o.Configuration = configuration.RedisConnectionString);
        }

        return services;
    }
}