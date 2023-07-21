namespace SFA.DAS.EmployerAccounts.Configuration;

public class ProviderRegistrationClientApiConfiguration : IProviderRegistrationClientApiConfiguration
{
    public string BaseUrl { get; set; }
    public string IdentifierUri { get; set; }
}