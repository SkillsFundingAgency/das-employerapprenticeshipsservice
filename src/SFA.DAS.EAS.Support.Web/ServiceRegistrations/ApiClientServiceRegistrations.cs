using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

namespace SFA.DAS.EAS.Support.Web.ServiceRegistrations;

public static class ApiClientServiceRegistrations
{
    public static IServiceCollection AddApiClientServices(this IServiceCollection services)
    {
        services.AddSingleton<IAccountApiClient, AccountApiClient>();
        services.AddSingleton<ILevyTokenHttpClientFactory, LevyTokenHttpClientFactory>();
        services.AddSingleton<ITokenServiceApiClient, TokenServiceApiClient>();
        
        return services;
    }
}

