using System;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.Factories
{
    public class RestServiceFactory : IRestServiceFactory
    {
        private readonly IRestClientFactory _clientFactory;

        public RestServiceFactory(IRestClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IRestService Create(string baseUrl)
        {
            var client = _clientFactory.Create(new Uri(baseUrl));

            return new RestService(client);
        }
    }
}
