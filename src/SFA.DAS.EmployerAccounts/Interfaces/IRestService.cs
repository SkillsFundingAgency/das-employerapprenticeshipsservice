using RestSharp;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IRestService
{
    IRestRequest Create(Method method, string url, string jsonBody = null, params KeyValuePair<string, string>[] segments);
    IRestRequest Create(string url, params KeyValuePair<string, string>[] segments);
    IRestResponse<T> Execute<T>(IRestRequest request) where T : new();
}