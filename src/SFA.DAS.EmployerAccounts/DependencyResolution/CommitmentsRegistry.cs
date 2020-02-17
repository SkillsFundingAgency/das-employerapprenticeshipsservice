using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Http;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class CommitmentsRegistry : Registry
    {
        public CommitmentsRegistry()
        {
            For<CommitmentsApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<CommitmentsApiClientConfiguration>(ConfigurationKeys.CommitmentsApiClient)).Singleton();
            For<ICommitmentsApiClientConfiguration>().Use(c => c.GetInstance<CommitmentsApiClientConfiguration>());
            For<IEmployerCommitmentApi>().Use<EmployerCommitmentApi>().Ctor<HttpClient>().Is(c => GetHttpClient(c));
            For<IValidationApi>().Use<ValidationApi>();
        }

        private HttpClient GetHttpClient(IContext context)
        {
            var config = context.GetInstance<CommitmentsApiClientConfiguration>();

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
