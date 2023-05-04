using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi.Http;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Account.Api.ServiceRegistrations;

public static class ClientServiceRegistrations
{
    public static IServiceCollection AddClientServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmployerAccountsApiHttpClientFactory, EmployerAccountsApiHttpClientFactory>();
        services.AddSingleton<IEmployerAccountsApiService, EmployerAccountsApiService>();
        services.AddSingleton<IEmployerFinanceApiHttpClientFactory, EmployerFinanceApiHttpClientFactory>();
        services.AddSingleton<IEmployerFinanceApiService, EmployerFinanceApiService>();

        return services;
    }
}
