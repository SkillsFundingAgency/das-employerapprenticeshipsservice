using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class ContentApiClientServiceRegistrations
{
    public static IServiceCollection AddContentApiClient(this IServiceCollection services, EmployerAccountsConfiguration employerAccountsConfiguration)
    {
        services.AddSingleton<ContentClientApiConfiguration>(employerAccountsConfiguration.ContentApi);

        services.AddSingleton<IContentClientApiConfiguration>(employerAccountsConfiguration.ContentApi);

        services.AddHttpClient<IContentApiClient, ContentApiClient>();
        services.Decorate<IContentApiClient, ContentApiClientWithCaching>();

        return services;
    }
}