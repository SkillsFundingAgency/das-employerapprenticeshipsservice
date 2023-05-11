using Microsoft.Extensions.Options;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.ServiceRegistrations
{
    public static class ConfigurationServiceRegistrations
    {
        public static IServiceCollection AddConfigurationSections(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            services.Configure<EmployerAccountsConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccounts));
            services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerAccountsConfiguration>>().Value);

            services.Configure<EmployerAccountsReadStoreConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccountsReadStore));
            services.AddSingleton(c => c.GetService<IOptions<EmployerAccountsReadStoreConfiguration>>().Value);

            return services;
        }
    }
}
