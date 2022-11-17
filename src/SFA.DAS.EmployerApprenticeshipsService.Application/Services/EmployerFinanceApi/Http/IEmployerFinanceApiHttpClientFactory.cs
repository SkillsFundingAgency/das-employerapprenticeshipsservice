using System.Net.Http;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http
{
    public interface IEmployerFinanceApiHttpClientFactory
    {
        HttpClient CreateHttpClient();
    }
}