using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class DataProtectionServiceRegistrations
{
    public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection(ConfigurationKeys.ServiceName).Get<EmployerAccountsConfiguration>();

        if (config != null
            && !string.IsNullOrEmpty(config.DataProtectionKeysDatabase)
            && !string.IsNullOrEmpty(config.RedisConnectionString))
        {
            var redisConnectionString = config.RedisConnectionString;
            var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

            var redis = ConnectionMultiplexer
                .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

            services.AddDataProtection()
                .SetApplicationName("das-employer")
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
        }

        return services;
    }
}
