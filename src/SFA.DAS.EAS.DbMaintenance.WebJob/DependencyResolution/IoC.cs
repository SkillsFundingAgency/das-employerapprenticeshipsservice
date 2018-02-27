using SFA.DAS.EAS.Infrastructure.Caching;
using StructureMap;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<DefaultRegistry>();
                c.AddRegistry<CacheRegistry>();
            });
        }
    }
}