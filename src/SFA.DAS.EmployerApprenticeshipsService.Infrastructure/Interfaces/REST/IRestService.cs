using System.Collections.Generic;
using RestSharp;

namespace SFA.DAS.EAS.Infrastructure.Interfaces.REST
{
    public interface IRestService
    {
        IRestRequest Create(Method method, string url, string jsonBody = null, params KeyValuePair<string, string>[] segments);
        IRestRequest Create(string url, params KeyValuePair<string, string>[] segments);
        IRestResponse<T> Execute<T>(IRestRequest request) where T : new();
    }
}
