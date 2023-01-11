namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IRestServiceFactory
{
    IRestService Create(string baseUrl);
}