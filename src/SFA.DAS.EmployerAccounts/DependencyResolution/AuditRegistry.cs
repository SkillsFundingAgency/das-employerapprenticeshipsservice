using SFA.DAS.Audit.Client;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class AuditRegistry : Registry
{
    public AuditRegistry()
    {
        var auditClient = For<IAuditApiClient>().Use(c =>
            c.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL)
                ? new StubAuditApiClient() as IAuditApiClient
                : new AuditApiClient(c.GetInstance<IAuditApiConfiguration>()) as IAuditApiClient);

        For<IAuditApiClient>().Use<AuditApiClientForSupportUser>()
            .Ctor<IAuditApiClient>().Is(auditClient);

        For<IAuditApiConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<AuditApiClientConfiguration>(ConfigurationKeys.AuditApi)).Singleton();

        For<IAuditMessageFactory>().Use<AuditMessageFactory>().Singleton();
    }
}