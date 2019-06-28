using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.Factories
{
    public class HttpServiceFactory : IHttpServiceFactory
    {
        public IHttpService Create(string clientId, string clientSecret, string identifierUri, string tenant)
        {
            var client = new HttpService(clientId, clientSecret, identifierUri, tenant);
            return client;
        }
    }
}
