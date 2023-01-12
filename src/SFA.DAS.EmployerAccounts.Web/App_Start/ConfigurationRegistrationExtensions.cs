using Microsoft.Extensions.Options;
using SFA.DAS.Audit.Client;
using SFA.DAS.Authorization.EmployerFeatures.Configuration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.ReadStore.Configuration;
using SFA.DAS.Hmrc.Configuration;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.ReferenceData.Api.Client;

namespace SFA.DAS.EmployerAccounts.Web;

public static class ConfigurationRegistrationExtensions
{
    public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        services.Configure<EmployerAccountsConfiguration>(configuration.GetSection(nameof(EmployerAccountsConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerAccountsConfiguration>>().Value);

        services.Configure<EmployerAccountsReadStoreConfiguration>(configuration.GetSection(nameof(EmployerAccountsReadStoreConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerAccountsReadStoreConfiguration>>().Value);

        services.Configure<ReferenceDataApiClientConfiguration>(configuration.GetSection(nameof(ReferenceDataApiClientConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReferenceDataApiClientConfiguration>>().Value);

        services.Configure<EmployerFeaturesConfiguration>(configuration.GetSection(nameof(EmployerFeaturesConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerFeaturesConfiguration>>().Value);

        services.Configure<AccountApiConfiguration>(configuration.GetSection(nameof(AccountApiConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<AccountApiConfiguration>>().Value);

        services.Configure<IdentityServerConfiguration>(configuration.GetSection(nameof(IdentityServerConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<IdentityServerConfiguration>>().Value);

        services.Configure<IAuditApiConfiguration>(configuration.GetSection(nameof(AuditApiClientConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<IAuditApiConfiguration>>().Value);

        services.Configure<IAccountApiConfiguration>(configuration.GetSection(nameof(AccountApiConfiguration)));
        services.AddSingleton<IAccountApiConfiguration, AccountApiConfiguration>();

        var config = configuration.GetSection(nameof(EmployerAccountsConfiguration)) as EmployerAccountsConfiguration;
        services.AddSingleton<IHmrcConfiguration>(_ => config.Hmrc);

        services.Configure<INotificationsApiClientConfiguration>(configuration.GetSection(nameof(NotificationsApiClientConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<NotificationsApiClientConfiguration>>().Value);

        services.Configure<IReferenceDataApiConfiguration>(configuration.GetSection(nameof(ReferenceDataApiClientConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ReferenceDataApiClientConfiguration>>().Value);
    }
}