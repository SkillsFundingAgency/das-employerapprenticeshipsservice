using System.Net.Http;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.EmployerAccounts.Web;

public static class NotificationsRegistrationExtensions
{
    public static void AddNotifications(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<INotificationsApi>(s =>
        {
            var config = s.GetService<NotificationsApiClientConfiguration>();
            var httpClient = GetHttpClient(config);
            return new NotificationsApi(httpClient, config);
        });
        
    }

    private static HttpClient GetHttpClient(INotificationsApiClientConfiguration config)
    {
        var httpClient = string.IsNullOrWhiteSpace(config.ClientId)
            ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config)).Build()
            : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config)).Build();

        return httpClient;
    }
}