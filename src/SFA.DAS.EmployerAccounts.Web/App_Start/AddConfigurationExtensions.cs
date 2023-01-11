using Microsoft.Extensions.Options;
using SFA.DAS.EAS.Account.Api.Client;

namespace SFA.DAS.EmployerAccounts.Web;

public static class AddConfigurationExtensions
{
    public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<EmployerAccountsConfiguration>(configuration.GetSection(nameof(EmployerAccountsConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerAccountsConfiguration>>().Value);

        services.Configure<AccountApiConfiguration>(configuration.GetSection(nameof(AccountApiConfiguration)));
        services.Configure<IdentityServerConfiguration>(configuration.GetSection(nameof(IdentityServerConfiguration)));

        services.AddSingleton<IAccountApiConfiguration, AccountApiConfiguration>();
    }
}