namespace SFA.DAS.EmployerAccounts.Factories;

public class HttpServiceFactory : IHttpServiceFactory
{       
    public IHttpService Create(string identifierUri, string clientId = "", string clientSecret = "", string tenant = "")
    {
        var client = new HttpService(clientId, clientSecret, identifierUri, tenant);
        return client;
    }
}