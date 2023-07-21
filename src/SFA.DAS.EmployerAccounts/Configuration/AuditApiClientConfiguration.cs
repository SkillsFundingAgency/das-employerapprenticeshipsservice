namespace SFA.DAS.EmployerAccounts.Configuration;

public class AuditApiClientConfiguration : IAuditApiClientConfiguration
{
    public string BaseUrl { get; set; }
    public string IdentifierUri { get; set; }
}