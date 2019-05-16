using SFA.DAS.AutoConfiguration;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.Portal.Client.Configuration;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            For<IPortalClientConfiguration>().Use(
                c => c.GetInstance<IAutoConfigurationService>()
                    .Get<PortalClientConfiguration>(ConfigurationKeys.PortalClient)).Singleton();
            For<ICosmosDbConfiguration>().Use(c => c.GetInstance<IPortalClientConfiguration>().CosmosDatabase).Singleton();
        }
    }
}