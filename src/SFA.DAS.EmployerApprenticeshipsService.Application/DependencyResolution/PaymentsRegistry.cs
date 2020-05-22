using SFA.DAS.AutoConfiguration;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using StructureMap;
using System.Net.Http;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class PaymentsRegistry : Registry
    {
        public PaymentsRegistry()
        {
            For<IPaymentsEventsApiConfiguration>().Use(c => c.GetInstance<PaymentsApiClientConfiguration>());
            For<PaymentsApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<PaymentsApiClientConfiguration>(ConfigurationKeys.PaymentsApiClient)).Singleton();
            For<IPaymentsEventsApiClient>().Use<PaymentsEventsApiClient>().Ctor<HttpClient>().Is(c => GetHttpClient(c));
        }

        private HttpClient GetHttpClient(IContext context)
        {
            var config = context.GetInstance<PaymentsApiClientConfiguration>();

            var httpClientBuilder = string.IsNullOrWhiteSpace(config.ClientId)
                ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config))
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config));

            return httpClientBuilder
                .WithDefaultHeaders()
                .WithHandler(new RequestIdMessageRequestHandler())
                .WithHandler(new SessionIdMessageRequestHandler())
                .Build();
        }
    }
}