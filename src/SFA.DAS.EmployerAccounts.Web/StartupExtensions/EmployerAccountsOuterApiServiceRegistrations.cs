using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi;
using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class EmployerAccountsOuterApiServiceRegistrations
{
    public static IServiceCollection AddEmployerAccountsOuterApi(this IServiceCollection services, EmployerAccountsConfiguration configuration)
    {
        services.AddScoped<IOuterApiClient, OuterApiClient>();

        services.AddHttpClient<IOuterApiClient, OuterApiClient>(x =>
        {
            x.BaseAddress = new Uri(configuration.EmployerAccountsOuterApiConfiguration.BaseUrl);
            x.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", configuration.EmployerAccountsOuterApiConfiguration.Key);
            x.DefaultRequestHeaders.Add("X-Version", "1");
        });

        return services;
    }
}