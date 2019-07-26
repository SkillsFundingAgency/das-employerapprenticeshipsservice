using SFA.DAS.Encoding;

namespace SFA.DAS.EAS.Portal.Client.Configuration
{
    public interface IPortalClientConfiguration
    {
        CosmosDatabaseConfiguration CosmosDatabase { get; }
        RecruitApiClientConfiguration RecruitApiClientConfiguration { get; }
    }
}