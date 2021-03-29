using System.Net.Http;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class ContentApiClientRegistry : Registry
    {
        public ContentApiClientRegistry()
        {
            For<ContentClientApiConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().ContentApi);
            For<IContentClientApiConfiguration>().Use(c => c.GetInstance<ContentClientApiConfiguration>());
            For<IContentApiClient>().Use<ContentApiClient>().Ctor<HttpClient>().Is(c => CreateClient(c));
            For<IContentApiClient>().DecorateAllWith<ContentApiClientWithCaching>().Ctor<HttpClient>().Is(c => CreateClient(c));
        }

        private HttpClient CreateClient(IContext context)
        {
            var config = context.GetInstance<EmployerAccountsConfiguration>().ContentApi;

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
