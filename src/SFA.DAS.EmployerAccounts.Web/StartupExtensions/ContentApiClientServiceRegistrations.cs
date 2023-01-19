using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class ContentApiClientServiceRegistrations
{
    public static IServiceCollection AddContentApiClient(this IServiceCollection services,
        EmployerAccountsConfiguration employerAccountsConfiguration, IConfiguration configuration)
    {
        services.AddSingleton(employerAccountsConfiguration.ContentApi);
        services.Configure<IContentClientApiConfiguration>(
            configuration.GetSection(nameof(ContentClientApiConfiguration)));
        services.AddHttpClient<IContentApiClient, ContentApiClient>()
            .AddHttpMessageHandler<RequestIdMessageRequestHandler>()
            .AddHttpMessageHandler<SessionIdMessageRequestHandler>();

        services.AddScoped<IContentApiClient, ContentApiClient>();
        services.Decorate<IContentApiClient, ContentApiClientWithCaching>();

        return services;
    }
}