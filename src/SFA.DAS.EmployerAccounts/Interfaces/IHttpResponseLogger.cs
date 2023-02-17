using System.Net.Http;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IHttpResponseLogger
{
    Task LogResponseAsync(HttpResponseMessage response);
}