namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IAuditApiClientConfiguration
{
    string BaseUrl { get; set; }
    string IdentifierUri { get; set; }
}
