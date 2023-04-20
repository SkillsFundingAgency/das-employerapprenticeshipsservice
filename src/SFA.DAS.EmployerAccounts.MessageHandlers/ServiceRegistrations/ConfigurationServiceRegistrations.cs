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

            services.Configure<EmployerAccountsReadStoreConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccountsReadStore));
            services.AddSingleton(c => c.GetService<IOptions<EmployerAccountsReadStoreConfiguration>>().Value);

            return services;
        }
    }
}
