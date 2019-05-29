
namespace SFA.DAS.EAS.Portal.Client.Configuration
{
    public class PortalClientConfiguration : IPortalClientConfiguration
    {
        public CosmosDatabaseConfiguration CosmosDatabase { get; set; }
    }
}