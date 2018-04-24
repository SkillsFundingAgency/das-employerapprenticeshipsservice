using SFA.DAS.Audit.Client;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class AuditRegistry : Registry
    {
        public AuditRegistry()
        {
            if (ConfigurationHelper.IsAnyOf(Environment.Local))
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