using SFA.DAS.Audit.Client;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EAS.Domain.Configuration;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class AuditRegistry : Registry
    {
        public AuditRegistry()
        {
            For<IAuditApiClient>().Use(c =>
                c.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL)
                    ? new StubAuditApiClient() as IAuditApiClient
                    : new AuditApiClient(c.GetInstance<IAuditApiConfiguration>()) as IAuditApiClient);

            For<IAuditApiConfiguration>().Use(c => c.GetInstance<AuditApiClientConfiguration>());
            For<IAuditMessageFactory>().Use<AuditMessageFactory>().Singleton();
        }
    }
}