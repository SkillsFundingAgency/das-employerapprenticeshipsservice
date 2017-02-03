using System;
using System.Collections.Generic;
using RestSharp;
using SFA.DAS.EAS.Infrastructure.Interfaces.REST;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class RestService : IRestService
    {
        private readonly IRestClient _client;

        public RestService(IRestClient client)
        {
            _client = client;
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
            var response = _client.Execute<T>(request);

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
