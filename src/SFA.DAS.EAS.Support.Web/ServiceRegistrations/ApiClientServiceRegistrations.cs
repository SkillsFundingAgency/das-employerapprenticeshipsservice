using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Application.Http;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.Web.ServiceRegistrations;

public static class ApiClientServiceRegistrations
{
    public static IServiceCollection AddApiClientServices(this IServiceCollection services)
    {
        services.AddTransient<ManagedIdentityTokenGenerator<EmployerAccountsApiConfiguration>>();
        
        services.AddSingleton<IAccountApiClient, AccountApiClient>();
        services.AddSingleton<ILevyTokenHttpClientFactory, LevyTokenHttpClientFactory>();
        services.AddSingleton<ITokenServiceApiClient, TokenServiceApiClient>();
        services.AddHttpClient<IEmployerAccountsApiService, EmployerAccountsApiService>();
        
        return services;
    }
}

