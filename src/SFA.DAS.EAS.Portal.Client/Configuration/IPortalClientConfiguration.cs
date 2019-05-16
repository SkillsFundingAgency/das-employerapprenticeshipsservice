using SFA.DAS.EAS.Portal.Configuration;

namespace SFA.DAS.EAS.Portal.Client.Configuration
{
    public interface IPortalClientConfiguration
    {
        CosmosDatabaseConfiguration CosmosDatabase { get; }
    }
}