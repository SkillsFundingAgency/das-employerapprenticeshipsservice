using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap
{
    public class PortalClientRegistry : Registry
    {
        public PortalClientRegistry()
        {
            IncludeRegistry<ApplicationRegistry>();
            IncludeRegistry<ReadStoreDataRegistry>();
        }
    }
}