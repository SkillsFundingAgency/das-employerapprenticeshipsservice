using HMRC.ESFA.Levy.Api.Client;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class ApprenticeshipLevyServiceRegistrations
{
    public static IServiceCollection AddApprenticeshipLevyClient(this IServiceCollection services, EmployerAccountsConfiguration configuration)
    {
        services.AddHttpClient<IApprenticeshipLevyApiClient, ApprenticeshipLevyApiClient>(client =>
        {
            client.BaseAddress = new Uri(configuration.Hmrc.BaseUrl);
        });

        services.AddScoped<IApprenticeshipLevyApiClient, ApprenticeshipLevyApiClient>();

        return services;
    }
}