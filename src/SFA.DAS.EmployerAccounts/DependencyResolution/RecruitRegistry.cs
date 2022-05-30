using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using SFA.DAS.Http;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using StructureMap;
using System.Net.Http;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class RecruitRegistry : Registry
    {
        public RecruitRegistry()
        {
            For<RecruitClientApiConfiguration>().Use(c => c.GetInstance<EmployerAccountsConfiguration>().RecruitApi);
            For<IRecruitClientApiConfiguration>().Use(c => c.GetInstance<RecruitClientApiConfiguration>());
            For<IRecruitService>().Use<RecruitService>().Ctor<HttpClient>().Is(c => CreateClient(c));            
            For<IRecruitService>().DecorateAllWith<RecruitServiceWithTimeout>();
        }

        private HttpClient CreateClient(IContext context)
        {
            HttpClient httpClient = new HttpClientBuilder()
                    .WithHandler(new RequestIdMessageRequestHandler())
                    .WithHandler(new SessionIdMessageRequestHandler())
                    .WithDefaultHeaders()
                    .Build();

            return httpClient;
        }
    }
}
