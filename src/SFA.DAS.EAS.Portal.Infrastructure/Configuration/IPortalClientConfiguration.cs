using SFA.DAS.EAS.DasRecruitService;

namespace SFA.DAS.EAS.Portal.Client.Configuration
{
    public interface IPortalClientConfiguration
    {
        CosmosDatabaseConfiguration CosmosDatabase { get; }

        RecruitApiClientConfiguration RecruitApiConfiguration { get; }
    }
}