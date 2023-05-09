using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Http;

namespace SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http;

public class EmployerFinanceApiHttpClientFactory : IEmployerFinanceApiHttpClientFactory
{
    private readonly EmployerFinanceApiConfiguration _employerFinanceApiConfig;

    public EmployerFinanceApiHttpClientFactory(EmployerFinanceApiConfiguration employerFinanceApiConfig)
    {
        _employerFinanceApiConfig = employerFinanceApiConfig;
    }

    public IRestHttpClient CreateHttpClient()
    {
        return new RestHttpClient(new ManagedIdentityHttpClientFactory(_employerFinanceApiConfig).CreateHttpClient());
    }
}