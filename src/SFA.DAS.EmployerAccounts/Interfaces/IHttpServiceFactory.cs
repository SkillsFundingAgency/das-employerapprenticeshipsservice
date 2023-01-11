namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IHttpServiceFactory
{
    IHttpService Create(string identifierUri, string clientId = "", string clientSecret = "", string tenant = "");
}