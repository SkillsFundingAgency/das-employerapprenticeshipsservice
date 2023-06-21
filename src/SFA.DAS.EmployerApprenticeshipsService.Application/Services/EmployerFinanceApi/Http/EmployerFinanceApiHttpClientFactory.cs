using System.Net.Http;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http;

public class EmployerFinanceApiHttpClientFactory : IEmployerFinanceApiHttpClientFactory
{
    private readonly EmployerFinanceApiConfiguration _employerFinanceApiConfig;

    public EmployerFinanceApiHttpClientFactory(EmployerFinanceApiConfiguration employerFinanceApiConfig)
    {
        _employerFinanceApiConfig = employerFinanceApiConfig;
    }

    public HttpClient CreateHttpClient() => new ManagedIdentityHttpClientFactory(_employerFinanceApiConfig).CreateHttpClient();
}
