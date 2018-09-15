using System.Net.Http;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class CommitmentsRegistry : Registry
    {
        public CommitmentsRegistry()
        {
            For<CommitmentsApiClientConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<CommitmentsApiClientConfiguration>("SFA.DAS.CommitmentsAPI")).Singleton();
            For<ICommitmentsApiClientConfiguration>().Use(c => c.GetInstance<CommitmentsApiClientConfiguration>());
            For<IEmployerCommitmentApi>().Use<EmployerCommitmentApi>().Ctor<HttpClient>().Is(c => GetHttpClient(c));
            For<IValidationApi>().Use<ValidationApi>();
        }

        private HttpClient GetHttpClient(IContext context)
        {
            var config = context.GetInstance<CommitmentsApiClientConfiguration>();

            var httpClient = new HttpClientBuilder()
                .WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config))
                .WithHandler(new RequestIdMessageRequestHandler())
                .WithHandler(new SessionIdMessageRequestHandler())
                .WithDefaultHeaders()
                .Build();

            return httpClient;
        }
    }
}
