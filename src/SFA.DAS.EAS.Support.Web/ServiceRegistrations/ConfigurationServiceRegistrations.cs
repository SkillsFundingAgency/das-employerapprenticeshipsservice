using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.EAS.Support.Web.Configuration;

namespace SFA.DAS.EAS.Support.Web.ServiceRegistrations;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddConfigurationSections(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<EasSupportConfiguration>(configuration);
        services.AddSingleton(cfg => cfg.GetService<IOptions<EasSupportConfiguration>>().Value);
        services.AddSingleton<IEasSupportConfiguration, EasSupportConfiguration>();
        services.AddSingleton<IAccountApiConfiguration>(sp => sp.GetService<IEasSupportConfiguration>().AccountApi);
        services.AddSingleton<ISiteValidatorSettings>(sp => sp.GetService<IEasSupportConfiguration>().SiteValidator);
        services.AddSingleton<IHmrcApiClientConfiguration>(sp => sp.GetService<IEasSupportConfiguration>().LevySubmission.HmrcApi);
        services.AddSingleton<ITokenServiceApiClientConfiguration>(sp => sp.GetService<IEasSupportConfiguration>().LevySubmission.TokenServiceApi);

        return services;
    }
}
