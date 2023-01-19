using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class AddDataProtectionExtensions
{
    public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {

        var config = configuration.GetSection(nameof(EmployerAccountsConfiguration))
            .Get<EmployerAccountsConfiguration>();

        if (config != null
            && !string.IsNullOrEmpty(config.DataProtectionKeysDatabase)
            && !string.IsNullOrEmpty(config.RedisConnectionString))
        {
            var redisConnectionString = config.RedisConnectionString;
            var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

            var redis = ConnectionMultiplexer
                .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

            services.AddDataProtection()
                .SetApplicationName("das-forecasting-web")
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
        }

        return services;
    }
}
