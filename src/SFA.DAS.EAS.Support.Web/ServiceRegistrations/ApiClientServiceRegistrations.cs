using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Clients;
using SFA.DAS.EAS.Support.Infrastructure.Services;
using SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;
using SFA.DAS.EmployerAccounts.Api.Client;

namespace SFA.DAS.EAS.Support.Web.ServiceRegistrations;

public static class ApiClientServiceRegistrations
{
    public static IServiceCollection AddApiClientServices(this IServiceCollection services)
    {
        services.AddSingleton<ISecureHttpClient, EmployerAccountsSecureHttpClient>();
        services.AddSingleton<ITokenServiceApiClient, TokenServiceApiClient>();
        services.AddSingleton<ILevyTokenHttpClientFactory, LevyTokenHttpClientFactory>();
        services.AddSingleton<IAccountApiClient, AccountApiClient>();

        return services;
    }
}

