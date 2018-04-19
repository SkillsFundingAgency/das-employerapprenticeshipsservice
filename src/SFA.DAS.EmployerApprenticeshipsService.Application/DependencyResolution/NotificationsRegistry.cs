using System.Net.Http;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class NotificationsRegistry : Registry
    {
        public NotificationsRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<Domain.Configuration.NotificationsApiClientConfiguration>($"{Constants.ServiceName}.Notifications");

            var httpClient = string.IsNullOrWhiteSpace(config.ClientId)
                ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config)).Build()
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureADBearerTokenGenerator(config)).Build();

            For<INotificationsApi>().Use<NotificationsApi>().Ctor<HttpClient>().Is(httpClient);
            For<INotificationsApiClientConfiguration>().Use(config);
        }
    }
}