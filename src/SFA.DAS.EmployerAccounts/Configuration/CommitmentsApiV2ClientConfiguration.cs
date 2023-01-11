namespace SFA.DAS.EmployerAccounts.Configuration;

public class CommitmentsApiV2ClientConfiguration : IManagedIdentityClientConfiguration
{
    public string ApiBaseUrl { get; set; }        
    public string IdentifierUri { get; set; }
}