using HMRC.ESFA.Levy.Api.Client;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

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

            return new ApprenticeshipLevyApiClient(client);
        });

        return services;
    }
}