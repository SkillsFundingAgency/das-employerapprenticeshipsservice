using System.Net.Http;

namespace SFA.DAS.EAS.Application.Http;

public interface IHttpClientFactory
{
    HttpClient CreateHttpClient();
}


