using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ActiveDirectory;
using SFA.DAS.EmployerAccounts.Infrastructure.Data;
using SFA.DAS.EmployerAccounts.Interfaces.Hmrc;
using SFA.DAS.TokenService.Api.Client;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class HmrcServiceRegistrations
{
    public static IServiceCollection AddHmrcServices(this IServiceCollection services)
    {
        services.AddScoped<IHmrcService, HmrcService>();
        services.AddSingleton<IHttpResponseLogger, HttpResponseLogger>();
        services.AddSingleton<ITokenServiceApiClient, TokenServiceApiClient>();
        services.AddSingleton<IAzureAdAuthenticationService, AzureAdAuthenticationService>();
        services.AddSingleton<IHttpClientWrapper, HttpClientWrapper>();

        return services;
    }
}