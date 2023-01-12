using HMRC.ESFA.Levy.Api.Client;
using Microsoft.Extensions.Options;
using System.Net.Http;
using SFA.DAS.Audit.Client;
using SFA.DAS.AutoConfiguration;

namespace SFA.DAS.EmployerAccounts.Web;

public static class AuditRegistrationExtensions
{
    public static void AddAuditServices(this IServiceCollection services)
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
    }
}