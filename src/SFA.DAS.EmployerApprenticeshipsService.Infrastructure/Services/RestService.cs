using System;
using System.Collections.Generic;
using RestSharp;
using SFA.DAS.EAS.Infrastructure.Interfaces.REST;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public abstract class RestService : IRestService
    {
        private readonly IRestClientFactory _restClientFactory;
        private IRestClient _client;

        public Uri BaseUrl { get; protected set; }

        protected RestService(IRestClientFactory restClientFactory)
            : this(restClientFactory, string.Empty)
        { }

        protected RestService(IRestClientFactory restClientFactory, string baseUrl)
        {
            _restClientFactory = restClientFactory;
            BaseUrl = string.IsNullOrEmpty(baseUrl) ? null : new Uri(baseUrl);
        }

        public IRestClient Client
        {
            get { return _client ?? (_client = _restClientFactory.Create(BaseUrl)); }

            set { _client = value; }
        }

        public virtual IRestRequest Create(
            Method method,
            string url,
            string jsonBody = null,
            params KeyValuePair<string, string>[] segments)
        {
            IRestRequest request = new RestRequest(url, method) { RequestFormat = DataFormat.Json };
            if (segments != null)
            {
                foreach (var segment in segments)
                {
                    request.AddUrlSegment(segment.Key, segment.Value);
                }
            }

            if (!string.IsNullOrEmpty(jsonBody))
            {
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
            }

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");

            return request;
        }

        public virtual IRestRequest Create(string url, params KeyValuePair<string, string>[] segments)
        {
            return Create(Method.GET, url, string.Empty, segments);
        }

        public virtual IRestResponse<T> Execute<T>(IRestRequest request) where T : new()
        {
            var response = Client.Execute<T>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                ThrowIncompleteResponseException(response);
            }

            if (response.ErrorException != null)
            {
                ThrowErrorException(response);
            }

            return response;
        }

        private static void ThrowIncompleteResponseException(IRestResponse response)
        {
            string message = $"REST service failed to complete: \"{response.ErrorMessage}\", response status: {response.ResponseStatus}, HTTP status code: {response.StatusCode}.";

            throw new ApplicationException(message);
        }

        private static void ThrowErrorException(IRestResponse response)
        {
            throw new ApplicationException("Error retrieving response. Check inner details for more info.",
                response.ErrorException);
        }
    }
}
