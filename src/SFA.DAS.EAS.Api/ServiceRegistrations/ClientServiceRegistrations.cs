using System;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.ServiceRegistrations;

public static class ClientServiceRegistrations
{
    public static IServiceCollection AddClientServices(this IServiceCollection services)
    {
        services.AddHttpClient<IEmployerAccountsApiService, EmployerAccountsApiService>((serviceProvider, client) =>
        {
            var config = serviceProvider.GetService<EmployerAccountsApiConfiguration>();
            client.BaseAddress = new Uri(config.ApiBaseUrl);
        });
        
        services.AddHttpClient<IEmployerFinanceApiService, EmployerFinanceApiService>((serviceProvider, client) =>
        {
            var config = serviceProvider.GetService<EmployerFinanceApiConfiguration>();
            client.BaseAddress = new Uri(config.ApiBaseUrl);
        });

        return services;
    }
}
