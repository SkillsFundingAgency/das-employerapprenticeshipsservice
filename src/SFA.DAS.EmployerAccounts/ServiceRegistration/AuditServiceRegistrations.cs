using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EmployerAccounts.Audit;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.ServiceRegistration;

public static class AuditServiceRegistrations
{
    public static IServiceCollection AddAuditServices(this IServiceCollection services, AuditApiClientConfiguration auditApiClientConfiguration)
    {
        services.AddSingleton<IAuditApiClientConfiguration>(auditApiClientConfiguration);

        services.AddHttpClient<IAuditApiClient, AuditApiClient>();
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}