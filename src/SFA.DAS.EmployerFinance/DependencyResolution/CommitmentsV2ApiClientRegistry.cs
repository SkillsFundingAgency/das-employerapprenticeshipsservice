using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.Http;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using StructureMap;
using System.Net.Http;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class CommitmentsV2ApiClientRegistry : Registry
    {
        public CommitmentsV2ApiClientRegistry()
        {          
            For<EmployerFinanceConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerFinanceConfiguration>(ConfigurationKeys.EmployerFinance)).Singleton();
            For<CommitmentsApiV2ClientConfiguration>().Use(c => c.GetInstance<EmployerFinanceConfiguration>().CommitmentsApi);

            For<ICommitmentsV2ApiClient>().Use<CommitmentsV2ApiClient>()
                .Ctor<HttpClient>().Is(c => GetHttpV2Client(c));
        }

        private HttpClient GetHttpV2Client(IContext context)
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
