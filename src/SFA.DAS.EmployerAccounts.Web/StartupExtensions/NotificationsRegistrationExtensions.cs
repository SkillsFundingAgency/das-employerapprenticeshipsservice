using System.Net.Http;
using Microsoft.Extensions.Options;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class NotificationsRegistrationExtensions
{
    public static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<INotificationsApiClientConfiguration>(configuration.GetSection(nameof(NotificationsApiClientConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<NotificationsApiClientConfiguration>>().Value);

        services.AddTransient<INotificationsApi>(s =>
        {
            var config = s.GetService<NotificationsApiClientConfiguration>();
            var httpClient = GetHttpClient(config);
            return new NotificationsApi(httpClient, config);
        });

        return services;
    }

    private static HttpClient GetHttpClient(INotificationsApiClientConfiguration config)
    {
        var httpClient = string.IsNullOrWhiteSpace(config.ClientId)
            ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config)).Build()
            : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config)).Build();

        return httpClient;
    }
}