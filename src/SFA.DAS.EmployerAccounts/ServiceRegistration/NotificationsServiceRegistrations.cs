using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class NotificationsServiceRegistrations
{
    public static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NotificationsApiClientConfiguration>(configuration.GetSection(ConfigurationKeys.NotificationsApiClient));
        services.AddSingleton<INotificationsApiClientConfiguration>(cfg => cfg.GetService<IOptions<NotificationsApiClientConfiguration>>().Value);

        services.AddTransient<INotificationsApi>(s =>
        {
            var config = s.GetService<INotificationsApiClientConfiguration>();
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