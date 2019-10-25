using System;
using System.Net.Http;
using SFA.DAS.EAS.Portal.Client.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.EAS.Portal.Client.Services.Recruit.Http
{
    public class RecruitApiHttpClientFactory : IRecruitApiHttpClientFactory
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
                .WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(_recruitApiClientConfig))
                .Build();

            httpClient.BaseAddress = new Uri(_recruitApiClientConfig.ApiBaseUrl);

            // set timeout, so we don't end up delaying the rendering of the homepage for a misbehaving api
            httpClient.Timeout = TimeSpan.Parse(_recruitApiClientConfig.TimeoutTimeSpan);

            return httpClient;
        }
    }
}