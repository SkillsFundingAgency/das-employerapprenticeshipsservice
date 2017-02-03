using System;
using RestSharp;
using RestSharp.Deserializers;
using SFA.DAS.EAS.Infrastructure.Interfaces.REST;

namespace SFA.DAS.EAS.Infrastructure.Factories
{
    public class RestClientFactory : IRestClientFactory
    {
        public IRestClient Create(Uri baseUrl)
        {
            var client = new RestClient { BaseUrl = baseUrl };
            client.AddHandler("application/json", new JsonDeserializer());

            return client;
        }
    }
}
