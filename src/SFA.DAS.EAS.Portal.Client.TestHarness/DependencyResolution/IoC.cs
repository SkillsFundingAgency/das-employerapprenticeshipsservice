using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.TestHarness.DependencyResolution
{
    public static class IoC
    {
        public static void Initialize(Registry registry)
        {
            registry.IncludeRegistry<AutoConfigurationRegistry>();
            registry.IncludeRegistry<ConfigurationRegistry>();
            registry.IncludeRegistry<PortalClientRegistry>();
        }
    }
}