using System.Net.Http;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http
{
    public interface IEmployerAccountsApiHttpClientFactory
    {
        HttpClient CreateHttpClient();
    }
}
