using SFA.DAS.CosmosDb;

namespace SFA.DAS.EAS.Portal.Client.Configuration
{
    public class CosmosDatabaseConfiguration : ICosmosDbConfiguration
    {
        public string Uri { get; set; }
        public string AuthKey { get; set; }
    }
}