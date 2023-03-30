﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Http;

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
            string accessToken = "";
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            try
            {
                accessToken = azureServiceTokenProvider.GetAccessTokenAsync(_employerAccountsApiConfig.IdentifierUri).Result;
            } 
            catch (Exception ex) 
            { 
                
            }

            var httpClient = new HttpClientBuilder()
                .WithDefaultHeaders()
                .Build();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            httpClient.BaseAddress = new Uri(_employerAccountsApiConfig.ApiBaseUrl);

            return httpClient;
        }
    }
}
