using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.Http;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http;

public class EmployerAccountsApiHttpClientFactory : IEmployerAccountsApiHttpClientFactory
{
    private readonly EmployerAccountsApiConfiguration _employerAccountsApiConfig;

    public EmployerAccountsApiHttpClientFactory(EmployerAccountsApiConfiguration employerAccountsApiConfig)
    {
        _employerAccountsApiConfig = employerAccountsApiConfig;
    }

    public IRestHttpClient CreateHttpClient()
    {
        return new RestHttpClient(new ManagedIdentityHttpClientFactory(_employerAccountsApiConfig).CreateHttpClient());
    }
}
