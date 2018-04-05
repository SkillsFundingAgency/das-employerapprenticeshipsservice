using SFA.DAS.Audit.Client;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class AuditRegistry : Registry
    {
        public AuditRegistry()
        {
            var environmentName = ConfigurationHelper.GetEnvironmentName();

            if (environmentName == "LOCAL")
            {
                For<IAuditApiClient>().Use<StubAuditApiClient>();
            }
            else
            {
                For<IAuditApiClient>().Use<AuditApiClient>();
            }

            For<IAuditMessageFactory>().Use<AuditMessageFactory>().Singleton();
        }
    }
}