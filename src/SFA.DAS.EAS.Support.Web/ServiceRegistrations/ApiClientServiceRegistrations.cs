using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.Web.ServiceRegistrations;

public static class ApiClientServiceRegistrations
{
    public static IServiceCollection AddApiClientServices(this IServiceCollection services)
    {
        services.AddHttpClient<IEmployerAccountsApiService, EmployerAccountsApiService>();
        services.AddHttpClient<IEmployerFinanceApiService, EmployerFinanceApiService>();
        
        services.AddSingleton<IAccountApiClient, AccountApiClient>();
        services.AddSingleton<ILevyTokenHttpClientFactory, LevyTokenHttpClientFactory>();
        services.AddSingleton<ITokenServiceApiClient, TokenServiceApiClient>();
        
        services.AddTransient<AzureServiceTokenProvider<EmployerAccountsApiConfiguration>>();
        services.AddTransient<AzureServiceTokenProvider<EmployerFinanceApiConfiguration>>();
        
        return services;
    }
}
