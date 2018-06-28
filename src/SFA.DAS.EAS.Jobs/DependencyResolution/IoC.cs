using SFA.DAS.EAS.Application.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Jobs.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<DataRegistry>();
                c.AddRegistry<NServiceBusRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}