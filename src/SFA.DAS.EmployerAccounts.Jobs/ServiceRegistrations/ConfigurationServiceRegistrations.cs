using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;

namespace SFA.DAS.EmployerAccounts.Jobs.ServiceRegistrations;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.Get<EmployerAccountsConfiguration>());
        
        services.Configure<EmployerAccountsReadStoreConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccountsReadStore));
        services.AddSingleton(configuration.GetSection(ConfigurationKeys.EmployerAccountsReadStore).Get<EmployerAccountsReadStoreConfiguration>());

        return services;
    }
}