using System;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.ServiceRegistrations;

public static class ClientServiceRegistrations
{
    public static IServiceCollection AddClientServices(this IServiceCollection services, EmployerApprenticeshipsServiceConfiguration configuration)
    {
        services.AddHttpClient<IEmployerAccountsApiService, EmployerAccountsApiService>(client =>
        {
            client.BaseAddress = new Uri(configuration.EmployerAccountsApi.ApiBaseUrl);
        }).ConfigurePrimaryHttpMessageHandler(_ =>
            new ManagedIdentityHeadersHandler(
                new ManagedIdentityTokenGenerator(configuration.EmployerAccountsApi)
                ));

        services.AddHttpClient<IEmployerFinanceApiService, EmployerFinanceApiService>(client =>
        {
            client.BaseAddress = new Uri(configuration.EmployerFinanceApi.ApiBaseUrl);
        }).ConfigurePrimaryHttpMessageHandler(_ =>
            new ManagedIdentityHeadersHandler(
                new ManagedIdentityTokenGenerator(configuration.EmployerFinanceApi)
                ));

        return services;
    }
}