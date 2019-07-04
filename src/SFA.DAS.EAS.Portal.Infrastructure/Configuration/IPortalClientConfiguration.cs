namespace SFA.DAS.EAS.Portal.Infrastructure.Configuration
{
    public interface IPortalClientConfiguration
    {
        CosmosDatabaseConfiguration CosmosDatabase { get; }

        RecruitApiClientConfiguration RecruitApiConfiguration { get; }
    }
}