using SFA.DAS.Audit.Client;

namespace SFA.DAS.EmployerAccounts.Configuration;

public class AuditApiClientConfiguration : IAuditApiConfiguration
{
    public string ApiBaseUrl { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string IdentifierUri { get; set; }
    public string Tenant { get; set; }
    public string DatabaseConnectionString { get; set; }
    public string ServiceBusConnectionString { get; set; }
    public string MessageServiceBusConnectionString { get; set; }

}