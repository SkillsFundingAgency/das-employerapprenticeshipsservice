using SFA.DAS.EmployerAccounts.Api.Client;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class EmployerAccountsApiClientServiceRegistrations
{
    public static IServiceCollection AddEmployerAccountsApi(this IServiceCollection services)
    {
        services.AddTransient<IEmployerAccountsApiClient, EmployerAccountsApiClient>();
        services.AddTransient<ISecureHttpClient, SecureHttpClient>();

        return services;
    }
}