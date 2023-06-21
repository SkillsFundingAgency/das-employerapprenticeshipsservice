using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Support.Infrastructure.Settings;
using SFA.DAS.EAS.Support.Web.Configuration;
using SFA.DAS.EmployerAccounts.Api.Client;

namespace SFA.DAS.EAS.Support.Web.ServiceRegistrations;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddConfigurationSections(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions();

        var supportConfig = configuration.GetSection(ConfigurationKeys.SupportEasConfig);

        services.Configure<EasSupportConfiguration>(supportConfig);
        services.Configure<IEmployerAccountsApiClientConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccountApiConfig));
        services.AddSingleton(cfg => cfg.GetService<IOptions<IEmployerAccountsApiClientConfiguration>>().Value);


        services.AddSingleton(cfg => cfg.GetService<IOptions<EasSupportConfiguration>>().Value);
        services.AddSingleton(cfg => cfg.GetService<IOptions<EasSupportConfiguration>>().Value);

        services.AddSingleton<IEasSupportConfiguration, EasSupportConfiguration>();
        services.AddSingleton<IAccountApiConfiguration>(sp => sp.GetService<EasSupportConfiguration>().AccountApi);
        services.AddSingleton<ISiteValidatorSettings>(sp => sp.GetService<EasSupportConfiguration>().SiteValidator);
        services.AddSingleton<IHmrcApiClientConfiguration>(sp => sp.GetService<EasSupportConfiguration>().LevySubmission.HmrcApi);
        services.AddSingleton<ITokenServiceApiClientConfiguration>(sp => sp.GetService<EasSupportConfiguration>().LevySubmission.TokenServiceApi);

        return services;
    }
}