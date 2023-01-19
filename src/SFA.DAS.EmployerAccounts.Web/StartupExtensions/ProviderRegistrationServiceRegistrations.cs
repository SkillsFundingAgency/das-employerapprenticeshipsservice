using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class ProviderRegistrationServiceRegistrations
{
    public static IServiceCollection AddProviderRegistration(this IServiceCollection services,
        EmployerAccountsConfiguration employerAccountsConfiguration, IConfiguration configuration)
    {
        services.AddSingleton(employerAccountsConfiguration.ProviderRegistrationsApi);
        
        services.Configure<IProviderRegistrationClientApiConfiguration>(
            configuration.GetSection(nameof(ProviderRegistrationClientApiConfiguration)));

        services.AddHttpClient<IProviderRegistrationApiClient, ProviderRegistrationApiClient>()
            .AddHttpMessageHandler<RequestIdMessageRequestHandler>()
            .AddHttpMessageHandler<SessionIdMessageRequestHandler>();

        services.AddScoped<IProviderRegistrationApiClient, ProviderRegistrationApiClient>();

        return services;
    }
}