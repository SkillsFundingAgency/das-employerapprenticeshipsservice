using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces.Hmrc;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;
using SFA.DAS.EmployerAccounts.TasksApi;
using SFA.DAS.Encoding;
using SFA.DAS.ReferenceData.Api.Client;
using SFA.DAS.TokenService.Api.Client;

namespace SFA.DAS.EmployerAccounts.Jobs.ServiceRegistrations;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.Get<EmployerAccountsConfiguration>());
        
        services.Configure<EmployerAccountsReadStoreConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccountsReadStore));
        services.AddSingleton(configuration.GetSection(ConfigurationKeys.EmployerAccountsReadStore).Get<EmployerAccountsReadStoreConfiguration>());

        //services.Configure<ReferenceDataApiClientConfiguration>(configuration.GetSection(ConfigurationKeys.ReferenceDataApiClient));
        //services.AddSingleton<IReferenceDataApiConfiguration>(cfg => cfg.GetService<IOptions<ReferenceDataApiClientConfiguration>>().Value);

        //services.AddSingleton<IAccountApiConfiguration>(c => c.GetService<EmployerAccountsConfiguration>().AccountApi);

        //services.Configure<IdentityServerConfiguration>(configuration.GetSection("Identity"));
        //services.AddSingleton(cfg => cfg.GetService<IOptions<IdentityServerConfiguration>>().Value);


        var encodingConfigJson = configuration.GetSection("SFA.DAS.Encoding").Value;
        var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
        services.AddSingleton(encodingConfig);


        //services.AddSingleton<IHmrcConfiguration>(c => c.GetService<EmployerAccountsConfiguration>().Hmrc);
        //services.AddSingleton<ITokenServiceApiClientConfiguration>(c => c.GetService<EmployerAccountsConfiguration>().TokenServiceApi);
        //services.AddSingleton<ITaskApiConfiguration>(c => c.GetService<EmployerAccountsConfiguration>().TasksApi);

        //services.Configure<IEmployerAccountsApiClientConfiguration>(configuration.GetSection(ConfigurationKeys.EmployerAccountsApiClient));
        //services.AddSingleton<IEmployerAccountsApiClientConfiguration>(cfg => cfg.GetService<IOptions<EmployerAccountsApiClientConfiguration>>().Value);

        return services;
    }
}