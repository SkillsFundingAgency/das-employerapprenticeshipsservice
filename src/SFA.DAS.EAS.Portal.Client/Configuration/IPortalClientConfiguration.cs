namespace SFA.DAS.EAS.Portal.Client.Configuration
{
    public interface IPortalClientConfiguration
    {
        CosmosDatabaseConfiguration CosmosDatabase { get; }
    }
}