//using Microsoft.Azure.Services.AppAuthentication;
//using SFA.DAS.EAS.Domain.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;
//using SFA.DAS.EmployerFinance.Api.Client;

//namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http
//{
//    public class EmployerFinanceApiHttpClientFactory : IEmployerFinanceApiHttpClientFactory
//    {
//        private readonly IEmployerFinanceApiClientConfiguration _employerFinanceApiConfiguration;
//        private readonly SFA.DAS.EmployerFinance.Api.Client.SecureHttpClient _httpClient;

//        public EmployerFinanceApiHttpClientFactory(IEmployerFinanceApiClientConfiguration employerFinanceApiConfiguration)
//        {
//            _employerFinanceApiConfiguration = employerFinanceApiConfiguration;
//            _httpClient = new SecureHttpClient(employerFinanceApiConfiguration);
//        }

//        public async HttpClient CreateHttpClient()
//        {
//            var accessToken = await GetManagedIdentityAuthenticationResult(_employerFinanceApiConfiguration.IdentifierUri);

//            var httpClient = new HttpClient();
//            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

//            return httpClient;

//        }

//        private async Task<string> GetManagedIdentityAuthenticationResult(string resource)
//        {
//            var azureServiceTokenProvider = new AzureServiceTokenProvider();
//            return await azureServiceTokenProvider.GetAccessTokenAsync(resource);
//        }
//    }
//}
