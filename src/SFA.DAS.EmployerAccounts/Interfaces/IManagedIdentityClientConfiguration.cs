namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IManagedIdentityClientConfiguration
{
    string ApiBaseUrl { get; }
    string IdentifierUri { get; }
}