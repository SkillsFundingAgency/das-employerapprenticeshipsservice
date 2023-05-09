using System.Net.Http;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http;

public class EmployerAccountsApiHttpClientFactory : IEmployerAccountsApiHttpClientFactory
{
    private readonly EmployerAccountsApiConfiguration _employerAccountsApiConfig;

    public EmployerAccountsApiHttpClientFactory(EmployerAccountsApiConfiguration employerAccountsApiConfig)
    {
        _employerAccountsApiConfig = employerAccountsApiConfig;
    }

    public HttpClient CreateHttpClient() => new ManagedIdentityHttpClientFactory(_employerAccountsApiConfig).CreateHttpClient();
}
