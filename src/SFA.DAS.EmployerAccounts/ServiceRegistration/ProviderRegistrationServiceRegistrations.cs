using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class ProviderRegistrationServiceRegistrations
{
    public static IServiceCollection AddProviderRegistration(this IServiceCollection services, EmployerAccountsConfiguration employerAccountsConfiguration)
    {
        services.AddSingleton<ProviderRegistrationClientApiConfiguration>(employerAccountsConfiguration.ProviderRegistrationsApi);

        services.AddSingleton<IProviderRegistrationClientApiConfiguration>(_ => employerAccountsConfiguration.ProviderRegistrationsApi);

        services.AddHttpClient<IProviderRegistrationApiClient, ProviderRegistrationApiClient>();

        return services;
    }
}