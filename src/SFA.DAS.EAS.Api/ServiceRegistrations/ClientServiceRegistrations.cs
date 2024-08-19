using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.ServiceRegistrations;

public static class ClientServiceRegistrations
{
    public static IServiceCollection AddClientServices(this IServiceCollection services)
    {
        services.AddTransient<AzureServiceTokenProvider<EmployerAccountsApiConfiguration>>();
        services.AddTransient<AzureServiceTokenProvider<EmployerFinanceApiConfiguration>>();
        
        services.AddHttpClient<IEmployerAccountsApiService, EmployerAccountsApiService>();
        services.AddHttpClient<IEmployerFinanceApiService, EmployerFinanceApiService>();

        return services;
    }
}