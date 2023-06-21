namespace SFA.DAS.EAS.Domain.Configuration;

public class EmployerAccountsApiConfiguration: IManagedIdentityClientConfiguration
{
    public string IdentifierUri { get; set; }
    public string ApiBaseUrl { get; set; }
}
