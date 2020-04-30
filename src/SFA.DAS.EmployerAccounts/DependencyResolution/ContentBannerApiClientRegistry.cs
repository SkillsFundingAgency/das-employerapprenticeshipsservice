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
    public class ContentBannerApiClientRegistry : Registry
    {
        public ContentBannerApiClientRegistry()
        {
            For<ContentBannerClientApiConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().ContentBannerApi);
            For<IContentBannerClientApiConfiguration>().Use(c => c.GetInstance<ContentBannerClientApiConfiguration>());
            For<IContentBannerApiClient>().Use<ContentBannerApiClient>().Ctor<HttpClient>().Is(c => CreateClient(c));
        }

        private HttpClient CreateClient(IContext context)
        {
            var config = context.GetInstance<EmployerAccountsConfiguration>().ContentBannerApi;

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
