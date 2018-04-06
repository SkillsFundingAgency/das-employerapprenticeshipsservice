using System.Net.Http;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class CommitmentsRegistry : Registry
    {
        public CommitmentsRegistry()
        {
            var config = ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(Constants.ServiceName);
            var bearerToken = (IGenerateBearerToken)new JwtBearerTokenGenerator(config.CommitmentsApi);

            var httpClient = new HttpClientBuilder()
                .WithBearerAuthorisationHeader(bearerToken)
                .WithHandler(new RequestIdMessageRequestHandler())
                .WithHandler(new SessionIdMessageRequestHandler())
                .WithDefaultHeaders()
                .Build();

            For<IEmployerCommitmentApi>().Use<EmployerCommitmentApi>()
                .Ctor<HttpClient>().Is(httpClient)
                .Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi);

            For<IValidationApi>().Use<ValidationApi>().Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi);
        }
    }
}