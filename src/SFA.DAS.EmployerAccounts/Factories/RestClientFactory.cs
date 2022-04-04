using System;
using RestSharp;
using RestSharp.Serialization.Json;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Factories
{
    public class RestClientFactory : IRestClientFactory
    {
        public IRestClient Create(Uri baseUrl)
        {
            var client = new RestClient { BaseUrl = baseUrl };
            client.AddHandler("application/json", () => new JsonSerializer());

            return client;
        }
    }
}
