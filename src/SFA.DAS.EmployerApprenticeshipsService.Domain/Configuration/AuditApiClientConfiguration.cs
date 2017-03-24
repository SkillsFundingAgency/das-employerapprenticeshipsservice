using SFA.DAS.Audit.Client;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Domain.Configuration
{
    public class AuditApiClientConfiguration : IAuditApiConfiguration, IConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}