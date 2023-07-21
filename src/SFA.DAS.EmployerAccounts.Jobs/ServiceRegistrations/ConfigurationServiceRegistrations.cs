using Microsoft.Extensions.Options;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;

namespace SFA.DAS.EmployerAccounts.Jobs.ServiceRegistrations;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmployerAccountsConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccounts));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerAccountsConfiguration>>().Value);

        services.Configure<EmployerAccountsReadStoreConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccountsReadStore));
        services.AddSingleton(c => c.GetService<IOptions<EmployerAccountsReadStoreConfiguration>>().Value);

        return services;
    }
}