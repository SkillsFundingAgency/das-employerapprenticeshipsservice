using SFA.DAS.Http;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http
{
    public interface IEmployerFinanceApiHttpClientFactory
    {
        IRestHttpClient CreateHttpClient();
    }
}