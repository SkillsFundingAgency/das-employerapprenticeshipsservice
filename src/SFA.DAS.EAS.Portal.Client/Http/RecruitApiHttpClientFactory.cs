using System;
using System.Net.Http;
using SFA.DAS.EAS.Portal.Client.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.EAS.Portal.Client.Http
{
    public class RecruitApiHttpClientFactory //: IHttpClientFactory
    {
        private readonly RecruitApiClientConfiguration _recruitApiClientConfig;

        public RecruitApiHttpClientFactory(RecruitApiClientConfiguration recruitApiClientConfig)
        {
            _recruitApiClientConfig = recruitApiClientConfig;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClientBuilder()
                .WithDefaultHeaders()
                .WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(_recruitApiClientConfig))
                .Build();

            httpClient.BaseAddress = new Uri(_recruitApiClientConfig.ApiBaseUrl);

            return httpClient;
        }
    }
}