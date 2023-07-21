using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EmployerAccounts.Interfaces.Hmrc;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;
using SFA.DAS.EmployerAccounts.TasksApi;
using SFA.DAS.Encoding;
using SFA.DAS.ReferenceData.Api.Client;
using SFA.DAS.TokenService.Api.Client;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<EmployerAccountsConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccounts));

        services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerAccountsConfiguration>>().Value);

        services.Configure<EmployerAccountsReadStoreConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccountsReadStore));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerAccountsReadStoreConfiguration>>().Value);

        services.Configure<ReferenceDataApiClientConfiguration>(configuration.GetSection(ConfigurationKeys.ReferenceDataApiClient));
        services.AddSingleton<IReferenceDataApiConfiguration>(cfg => cfg.GetService<IOptions<ReferenceDataApiClientConfiguration>>().Value);

        services.AddSingleton<IAccountApiConfiguration>(cfg => cfg.GetService<EmployerAccountsConfiguration>().AccountApi);

        services.Configure<IdentityServerConfiguration>(configuration.GetSection("Identity"));
        services.AddSingleton(cfg => cfg.GetService<IOptions<IdentityServerConfiguration>>().Value);


        var encodingConfigJson = configuration.GetSection(ConfigurationKeys.EncodingConfig).Value;
        var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
        services.AddSingleton(encodingConfig);

        services.AddSingleton<IHmrcConfiguration>(cfg => cfg.GetService<EmployerAccountsConfiguration>().Hmrc);
        services.AddSingleton<ITokenServiceApiClientConfiguration>(cfg => cfg.GetService<EmployerAccountsConfiguration>().TokenServiceApi);
        services.AddSingleton<ITaskApiConfiguration>(cfg => cfg.GetService<EmployerAccountsConfiguration>().TasksApi);

        services.Configure<IEmployerAccountsApiClientConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccountsApiClient));
        services.AddSingleton<IEmployerAccountsApiClientConfiguration>(cfg => cfg.GetService<IOptions<EmployerAccountsApiClientConfiguration>>().Value);

        return services;
    }
}