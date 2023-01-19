using Microsoft.Extensions.Options;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class EmployerAccountsOuterApiServiceRegistrations
{
    public static IServiceCollection AddEmployerAccountsOuterApi(this IServiceCollection services, EmployerAccountsConfiguration employerAccountsConfiguration, IConfiguration configuration)
    {
        services.Configure<EmployerAccountsOuterApiConfiguration>(configuration.GetSection(nameof(EmployerAccountsOuterApiConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<EmployerAccountsOuterApiConfiguration>>().Value);

        services.AddScoped<IOuterApiClient, OuterApiClient>();

        services.AddHttpClient<IOuterApiClient, OuterApiClient>(x =>
        {
            x.BaseAddress = new Uri(employerAccountsConfiguration.EmployerAccountsOuterApiConfiguration.BaseUrl);
            x.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", employerAccountsConfiguration.EmployerAccountsOuterApiConfiguration.Key);
            x.DefaultRequestHeaders.Add("X-Version", "1");
        });

        return services;
    }
}