using HMRC.ESFA.Levy.Api.Client;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class ApprenticeshipLevyApiServiceRegistrations
{
    public static IServiceCollection AddApprenticeshipLevyApi(this IServiceCollection services, EmployerAccountsConfiguration employerAccountsConfiguration)
    {
        services.AddHttpClient<IApprenticeshipLevyApiClient, ApprenticeshipLevyApiClient>(client =>
        {
            if (!employerAccountsConfiguration.Hmrc.BaseUrl.EndsWith("/"))
            {
                employerAccountsConfiguration.Hmrc.BaseUrl += "/";
            }
            client.BaseAddress = new Uri(employerAccountsConfiguration.Hmrc.BaseUrl);
        });

        services.AddScoped<IApprenticeshipLevyApiClient, ApprenticeshipLevyApiClient>();

        return services;
    }
}