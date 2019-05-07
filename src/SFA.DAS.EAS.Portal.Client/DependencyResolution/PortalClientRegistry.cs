using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.DependencyResolution
{
    public class PortalClientRegistry : Registry
    {
        public PortalClientRegistry()
        {
            IncludeRegistry<ReadStoreDataRegistry>();
        }
    }
}