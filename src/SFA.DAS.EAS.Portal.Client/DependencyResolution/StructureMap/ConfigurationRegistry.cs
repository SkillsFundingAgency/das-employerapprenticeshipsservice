using SFA.DAS.AutoConfiguration;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Configuration;
using SFA.DAS.Encoding;
using StructureMap;
using ConfigurationKeys = SFA.DAS.EAS.Portal.Client.Configuration.ConfigurationKeys;
using IPortalClientConfiguration = SFA.DAS.EAS.Portal.Client.Configuration.IPortalClientConfiguration;

namespace SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            For<IPortalClientConfiguration>().Use(
                c => c.GetInstance<IAutoConfigurationService>()
                    .Get<PortalClientConfiguration>(ConfigurationKeys.PortalClient)).Singleton();
            For<ICosmosDbConfiguration>()
                .Use(c => c.GetInstance<IPortalClientConfiguration>().CosmosDatabase).Singleton();
            For<RecruitApiClientConfiguration>()
                .Use(c => c.GetInstance<IPortalClientConfiguration>().RecruitApiClientConfiguration).Singleton();
            
            For<EncodingConfig>().Use(
                c => c.GetInstance<IAutoConfigurationService>()
                    .Get<EncodingConfig>(ConfigurationKeys.Encoding)).Singleton();
        }
    }
}