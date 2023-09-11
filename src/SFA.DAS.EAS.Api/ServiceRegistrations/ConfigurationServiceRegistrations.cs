using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Account.Api.ServiceRegistrations;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddApiConfigurationSections(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        var easConfiguration = configuration.Get<EmployerApprenticeshipsServiceConfiguration>();
        services.AddSingleton(easConfiguration);

        services.AddSingleton(sp => sp.GetService<EmployerApprenticeshipsServiceConfiguration>().EmployerAccountsApi);
        services.AddSingleton(sp => sp.GetService<EmployerApprenticeshipsServiceConfiguration>().EmployerFinanceApi);

        var encodingConfigJson = configuration.GetSection(ConfigurationKeys.EncodingConfig).Value;
        var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
        services.AddSingleton(encodingConfig);

        return services;
    }
}
