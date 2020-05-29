using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using StructureMap;
using System.Net.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using SFA.DAS.Provider.Events.Api.Client;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class PaymentsRegistry : Registry
    {
        public PaymentsRegistry()
        {
            For<PaymentsApiClientConfiguration>().Use(c => c.GetInstance<EmployerFinanceConfiguration>().PaymentsEventsApi);
            For<IPaymentsEventsApiConfiguration>().Use(c => c.GetInstance<PaymentsApiClientConfiguration>());
            For<IPaymentsEventsApiClient>().Use<PaymentsEventsApiClient>().Ctor<HttpClient>().Is(c => CreateClient(c));
        }

        private HttpClient CreateClient(IContext context)
        {
            var config = context.GetInstance<EmployerFinanceConfiguration>().PaymentsEventsApi;

            HttpClient httpClient = new HttpClientBuilder()
                    .WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config))
                    .WithHandler(new RequestIdMessageRequestHandler())
                    .WithHandler(new SessionIdMessageRequestHandler())
                    .WithDefaultHeaders()
                    .Build();


            return httpClient;
        }
    }
}