using System;
using System.Net.Http;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.EAS.Portal.Application.Services.Commitments.Http
{
    public class CommitmentsApiHttpClientFactory : ICommitmentsApiHttpClientFactory
    {
        private readonly CommitmentsApiClientConfiguration _commitmentsApiClientConfiguration;

        public CommitmentsApiHttpClientFactory(CommitmentsApiClientConfiguration commitmentsApiClientConfiguration)
        {
            _commitmentsApiClientConfiguration = commitmentsApiClientConfiguration;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClientBuilder()
                .WithDefaultHeaders()
                .WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(_commitmentsApiClientConfiguration))
                .Build();

            httpClient.BaseAddress = new Uri(_commitmentsApiClientConfiguration.BaseUrl);

            return httpClient;
        }
    }
}