using System;
using System.Net.Http;
using System.Net.Http.Headers;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Http;
using Azure.Identity;
using Azure.Core;

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
            var tokenCredential = new DefaultAzureCredential();
            var accessToken = tokenCredential.GetToken(
                new TokenRequestContext(scopes: new string[] { _employerFinanceApiConfig.IdentifierUri + "/.default" }) { }
            ).Token;


            var httpClient = new HttpClientBuilder()
                .WithDefaultHeaders()
                .Build();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            httpClient.BaseAddress = new Uri(_employerFinanceApiConfig.ApiBaseUrl);

            return httpClient;
        }
    }
}