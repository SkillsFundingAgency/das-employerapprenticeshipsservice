using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Http;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http
{
    public class EmployerFinanceApiHttpClientFactory : IEmployerFinanceApiHttpClientFactory
    {
        private readonly EmployerFinanceApiConfiguration _employerFinanceApiConfig;

        public EmployerFinanceApiHttpClientFactory(EmployerFinanceApiConfiguration employerFinanceApiConfig)
        {
            _employerFinanceApiConfig = employerFinanceApiConfig;
        }

        public HttpClient CreateHttpClient()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = azureServiceTokenProvider.GetAccessTokenAsync(_employerFinanceApiConfig.IdentifierUri).Result;

            var httpClient = new HttpClientBuilder()
                .WithDefaultHeaders()
                .Build();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            httpClient.BaseAddress = new Uri(_employerFinanceApiConfig.ApiBaseUrl);

            return httpClient;
        }
    }
}