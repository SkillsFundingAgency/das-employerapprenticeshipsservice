namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IHttpServiceFactory
    {
        IHttpService Create(string clientId, string clientSecret, string identifierUri, string tenant);
    }
}