using SFA.DAS.EAS.Portal.Client.DependencyResolution.StructureMap;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.TestHarness.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<PortalClientRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<ScenariosRegistry>();
            });
        }
    }
}