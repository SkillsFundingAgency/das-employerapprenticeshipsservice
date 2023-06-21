namespace SFA.DAS.EAS.Domain.Configuration;

public interface IManagedIdentityClientConfiguration
{
    string ApiBaseUrl { get; }
    string IdentifierUri { get; }
}


