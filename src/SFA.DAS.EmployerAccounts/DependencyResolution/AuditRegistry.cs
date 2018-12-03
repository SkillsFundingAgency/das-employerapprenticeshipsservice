using SFA.DAS.Audit.Client;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerAccounts.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.DependencyResolution
{
    public class AuditRegistry : Registry
    {
        public AuditRegistry()
        {
            For<AuditApiClientConfiguration>().Use(() => ConfigurationHelper.GetConfiguration<AuditApiClientConfiguration>("SFA.DAS.AuditApiClient")).Singleton();

            if (ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local))
            {
                For<IAuditApiClient>().Use<StubAuditApiClient>();
            }
            else
            {
                For<IAuditApiClient>().Use<AuditApiClient>();
            }

            For<IAuditApiConfiguration>().Use(c => c.GetInstance<AuditApiClientConfiguration>());
            For<IAuditMessageFactory>().Use<AuditMessageFactory>().Singleton();
        }
    }
}