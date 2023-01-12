using Microsoft.Extensions.Options;
using SFA.DAS.Authorization.EmployerFeatures.Configuration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;
using SFA.DAS.Hmrc.Configuration;

namespace SFA.DAS.EmployerAccounts.Web;

public static class ConfigurationRegistrationExtensions
{
    public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<EmployerAccountsConfiguration>(configuration.GetSection(nameof(EmployerAccountsConfiguration)));
        services.Configure<EmployerAccountsReadStoreConfiguration>(configuration.GetSection(nameof(EmployerAccountsReadStoreConfiguration)));
        services.Configure<ReferenceDataApiClientConfiguration>(configuration.GetSection(nameof(ReferenceDataApiClientConfiguration)));
        services.Configure<EmployerFeaturesConfiguration>(configuration.GetSection(nameof(EmployerFeaturesConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerAccountsConfiguration>>().Value);

        services.Configure<AccountApiConfiguration>(configuration.GetSection(nameof(AccountApiConfiguration)));
        services.Configure<IdentityServerConfiguration>(configuration.GetSection(nameof(IdentityServerConfiguration)));

        services.AddSingleton<IAccountApiConfiguration, AccountApiConfiguration>();

        var config = configuration.GetSection(nameof(EmployerAccountsConfiguration)) as EmployerAccountsConfiguration;

        services.AddSingleton<IHmrcConfiguration>(_ => config.Hmrc);

        // IncludeRegistry<AutoConfigurationRegistry>();
        //For<EmployerAccountsConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsConfiguration>(ConfigurationKeys.EmployerAccounts)).Singleton();
        // For<EmployerAccountsReadStoreConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerAccountsReadStoreConfiguration>(ConfigurationKeys.EmployerAccountsReadStore)).Singleton();
        // For<ReferenceDataApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<ReferenceDataApiClientConfiguration>(ConfigurationKeys.ReferenceDataApiClient)).Singleton();
        // For<IAccountApiConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().AccountApi).Singleton();
        // For<EmployerFeaturesConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerFeaturesConfiguration>(ConfigurationKeys.Features)).Singleton();
        //  For<IHmrcConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().Hmrc).Singleton();
        // For<EncodingConfig>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EncodingConfig>(ConfigurationKeys.EncodingConfig)).Singleton();
    }
}