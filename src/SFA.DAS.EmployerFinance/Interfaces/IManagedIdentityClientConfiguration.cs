namespace SFA.DAS.EmployerFinance.Interfaces
{    
    public interface IManagedIdentityClientConfiguration
    {
        string ApiBaseUrl { get; }
        string IdentifierUri { get; }
    }
}
