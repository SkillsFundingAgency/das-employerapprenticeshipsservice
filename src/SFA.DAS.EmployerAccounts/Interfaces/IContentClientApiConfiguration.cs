using SFA.DAS.Http.Configuration;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IContentClientApiConfiguration : IManagedIdentityClientConfiguration
    {
    }

    public interface IManagedIdentityClientConfiguration
    {
        string ApiBaseUrl { get; }
        string IdentifierUri { get; }
    }
}
