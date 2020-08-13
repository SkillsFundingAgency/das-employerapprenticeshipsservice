//using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class ContentApiClientRegistry : Registry
    {
        public ContentApiClientRegistry()
        {
            For<ContentClientApiConfiguration>().Use(c => c.GetInstance<EmployerFinanceConfiguration>().ContentApi);
            For<IContentClientApiConfiguration>().Use(c => c.GetInstance<ContentClientApiConfiguration>());
            For<IClientContentApiClient>().Use<ClientContentApiClient>().Ctor<HttpClient>().Is(c => CreateClient(c));
        }
        private HttpClient CreateClient(IContext context)
        {
            var config = context.GetInstance<EmployerFinanceConfiguration>().ContentApi;
  
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
