using SFA.DAS.Audit.Client;
using SFA.DAS.AutoConfiguration;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class AuditServiceRegistrations
{
    public static IServiceCollection AddAuditServices(this IServiceCollection services)
    {
        services.AddScoped(s =>
               {
                   var environment = s.GetService<IEnvironmentService>();
                   var client = environment.IsCurrent(DasEnv.LOCAL)
                        ? new StubAuditApiClient()
                        : new AuditApiClient(s.GetService<IAuditApiConfiguration>()) as IAuditApiClient;

                   return client;
               });

        services.AddSingleton<IAuditMessageFactory, AuditMessageFactory>();

        return services;
    }
}