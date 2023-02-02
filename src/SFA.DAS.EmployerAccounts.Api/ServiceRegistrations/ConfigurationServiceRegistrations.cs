using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Api.ServiceRegistrations;

public static  class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddApiConfigurationSections(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<EmployerAccountsConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccounts));

        var employerAccountsConfiguration = configuration.Get<EmployerAccountsConfiguration>();
        services.AddSingleton(employerAccountsConfiguration);
        
        services.Configure<EncodingConfig>(configuration.GetSection(ConfigurationKeys.EncodingConfig));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EncodingConfig>>().Value);
        
        return services;
    }
}