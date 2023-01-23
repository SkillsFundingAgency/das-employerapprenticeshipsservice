using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

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