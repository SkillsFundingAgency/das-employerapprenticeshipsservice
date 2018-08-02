﻿using System.Net.Http;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class NotificationsRegistry : Registry
    {
        public NotificationsRegistry()
        {
            For<INotificationsApi>().Use<NotificationsApi>().Ctor<HttpClient>().Is(c => GetHttpClient(c));
            For<INotificationsApiClientConfiguration>().Use(c => c.GetInstance<NotificationsApiClientConfiguration>());
            For<NotificationsApiClientConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<NotificationsApiClientConfiguration>("SFA.DAS.Notifications")).Singleton();
        }

        private HttpClient GetHttpClient(IContext context)
        {
            var config = context.GetInstance<NotificationsApiClientConfiguration>();

            var httpClient = string.IsNullOrWhiteSpace(config.ClientId)
                ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config)).Build()
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureADBearerTokenGenerator(config)).Build();

            return httpClient;
        }
    }
}