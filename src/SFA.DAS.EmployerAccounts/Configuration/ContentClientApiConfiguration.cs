namespace SFA.DAS.EmployerAccounts.Configuration;

public class ContentClientApiConfiguration : IContentClientApiConfiguration
{
    public string ApiBaseUrl { get; set; }     
    public string IdentifierUri { get; set; }
}