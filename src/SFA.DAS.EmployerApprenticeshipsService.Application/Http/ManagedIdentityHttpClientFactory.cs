using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.Http;

public class ManagedIdentityHttpClientFactory 
{
    private readonly IManagedIdentityClientConfiguration _configuration;

    private readonly ILoggerFactory _loggerFactory;

    public ManagedIdentityHttpClientFactory(IManagedIdentityClientConfiguration configuration)
        : this(configuration, null)
    {
    }

    public ManagedIdentityHttpClientFactory(IManagedIdentityClientConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }

    public HttpClient CreateHttpClient()
    {
        HttpClientBuilder httpClientBuilder = new();
        if (_loggerFactory != null)
        {
            httpClientBuilder.WithLogging(_loggerFactory);
        }

        HttpClient httpClient = httpClientBuilder.WithDefaultHeaders().WithManagedIdentityAuthorisationHeader(new ManagedIdentityTokenGenerator(_configuration)).Build();
        httpClient.BaseAddress = new Uri(_configuration.ApiBaseUrl);
        return httpClient;
    }
}


