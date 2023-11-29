using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Web.StartupExtensions;

public static class ConfigurationServiceExtensions
{
    public static IServiceCollection AddConfigurationSections(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        
        services.Configure<IdentityServerConfiguration>(configuration.GetSection("Identity"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<IdentityServerConfiguration>>().Value);

        services.Configure<EmployerApprenticeshipsServiceConfiguration>(configuration);
        services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerApprenticeshipsServiceConfiguration>>().Value);
        
        services.AddSingleton(sp => sp.GetService<EmployerApprenticeshipsServiceConfiguration>().EmployerAccountsOuterApiConfiguration);
        
        return services;
    }
}