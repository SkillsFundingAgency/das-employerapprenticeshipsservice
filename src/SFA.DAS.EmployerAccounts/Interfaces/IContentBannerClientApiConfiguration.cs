using SFA.DAS.Http.Configuration;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IContentBannerClientApiConfiguration : IAzureActiveDirectoryClientConfiguration
    {
        bool UseStub { get; set; }
    }
}
