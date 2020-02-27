using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Http;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class CommitmentsApiClientRegistry : Registry
    {     
        public CommitmentsApiClientRegistry()
        {
            For<ICommitmentsApiClient>().Use(c => c.GetInstance<ICommitmentsApiClientFactory>().CreateClient()).Singleton();
            For<ICommitmentsApiClientFactory>().Use<CommitmentsApiClientFactory>();
            For<IRestHttpClient>().Use<RestHttpClient>();
        }
    }

    public class CommitmentsApiClientFactory : ICommitmentsApiClientFactory
    {
        private readonly EmployerAccountsConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public CommitmentsApiClientFactory(EmployerAccountsConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public ICommitmentsApiClient CreateClient()
        {
            var httpClientFactory = new AzureActiveDirectoryHttpClientFactory(_configuration.CommitmentsApi, _loggerFactory);
            var httpClient = httpClientFactory.CreateHttpClient();
            var restHttpClient = new CommitmentsRestHttpClient(httpClient, _loggerFactory);
            var apiClient = new CommitmentsApiClient(restHttpClient);

            return apiClient;
        }
    }

    public class AzureActiveDirectoryHttpClientFactory : IHttpClientFactory
    {
        private readonly IAzureActiveDirectoryClientConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public AzureActiveDirectoryHttpClientFactory(IAzureActiveDirectoryClientConfiguration configuration)
            : this(configuration, null)
        {
        }

        public AzureActiveDirectoryHttpClientFactory(IAzureActiveDirectoryClientConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClientBuilder = new HttpClientBuilder();

            if (_loggerFactory != null)
            {
                httpClientBuilder.WithLogging(_loggerFactory);
            }

            var httpClient = httpClientBuilder
                .WithDefaultHeaders()
                .WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(_configuration))
                .Build();

            httpClient.BaseAddress = new Uri(_configuration.ApiBaseUrl);

            return httpClient;
        }
    }


    public class CommitmentsRestHttpClient : RestHttpClient
    {
        private readonly ILogger<CommitmentsRestHttpClient> _logger;

        public CommitmentsRestHttpClient(HttpClient httpClient, ILoggerFactory loggerFactory) : base(httpClient)
        {
            _logger = loggerFactory.CreateLogger<CommitmentsRestHttpClient>();
        }

        protected override Exception CreateClientException(HttpResponseMessage httpResponseMessage, string content)
        {
            return httpResponseMessage.StatusCode == HttpStatusCode.BadRequest && httpResponseMessage.GetSubStatusCode() == CommitmentsV2.Api.Types.Http.HttpSubStatusCode.DomainException
                ? CreateApiModelException(httpResponseMessage, content)
                : base.CreateClientException(httpResponseMessage, content);
        }

        private Exception CreateApiModelException(HttpResponseMessage httpResponseMessage, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning($"{httpResponseMessage.RequestMessage.RequestUri} has returned an empty string when an array of error responses was expected.");
                return new CommitmentsApiModelException(new List<ErrorDetail>());
            }

            var errors = new CommitmentsApiModelException(JsonConvert.DeserializeObject<ErrorResponse>(content).Errors);

            var errorDetails = string.Join(";", errors.Errors.Select(e => $"{e.Field} ({e.Message})"));
            _logger.Log(errors.Errors.Count == 0 ? LogLevel.Warning : LogLevel.Debug, $"{httpResponseMessage.RequestMessage.RequestUri} has returned {errors.Errors.Count} errors: {errorDetails}");

            return errors;
        }
    }
}
