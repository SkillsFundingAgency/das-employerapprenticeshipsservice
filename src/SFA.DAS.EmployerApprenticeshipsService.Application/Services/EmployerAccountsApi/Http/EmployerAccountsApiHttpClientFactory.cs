using System;
using System.Net.Http;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http
{
    public class EmployerAccountsApiHttpClientFactory : IEmployerAccountsApiHttpClientFactory
    {
        private readonly EmployerAccountsApiConfiguration _employerAccountsApiConfig;

        public EmployerAccountsApiHttpClientFactory(EmployerAccountsApiConfiguration employerAccountsApiConfig)
        {
            _employerAccountsApiConfig = employerAccountsApiConfig;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClientBuilder()
                .WithDefaultHeaders()
                .WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(_employerAccountsApiConfig))
                .Build();

            httpClient.BaseAddress = new Uri(_employerAccountsApiConfig.BaseUrl);

            httpClient.Timeout = TimeSpan.Parse(_employerAccountsApiConfig.TimeoutTimeSpan);

            return httpClient;
        }
    }
}
