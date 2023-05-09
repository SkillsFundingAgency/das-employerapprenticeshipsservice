using System.Net.Http;
using SFA.DAS.Http;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http
{
    public interface IEmployerAccountsApiHttpClientFactory
    {
        IRestHttpClient CreateHttpClient();
    }
}
