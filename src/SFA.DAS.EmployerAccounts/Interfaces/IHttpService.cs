using System.Net.Http;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IHttpService
{
    Task<string> GetAsync(string url, bool exceptionOnNotFound = true);
    Task<string> GetAsync(string url, Func<HttpResponseMessage, bool> responseChecker);
}