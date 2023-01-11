using System.Net.Http;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class NotificationsRegistry : Registry
{
    public NotificationsRegistry()
    {
        For<INotificationsApi>().Use<NotificationsApi>().Ctor<HttpClient>().Is(c => GetHttpClient(c));
        For<INotificationsApiClientConfiguration>().Use(c => c.GetInstance<NotificationsApiClientConfiguration>());
        For<NotificationsApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<NotificationsApiClientConfiguration>(ConfigurationKeys.NotificationsApiClient)).Singleton();

    }

    private static HttpClient GetHttpClient(IContext context)
    {
        var config = context.GetInstance<NotificationsApiClientConfiguration>();

        var httpClient = string.IsNullOrWhiteSpace(config.ClientId)
            ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config)).Build()
            : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config)).Build();

        return httpClient;
    }
}