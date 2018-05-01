using System.Diagnostics;
using SFA.DAS.EAS.Account.Worker.Infrastructure;
using SFA.DAS.EAS.Application.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Account.Worker.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<DefaultRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<MessagePublisherRegistry>();
                c.AddRegistry<ValidationRegistry>();
                c.ForConcreteType<DasWebJobTraceWriter>().Configure.Ctor<TraceLevel>().Is(TraceLevel.Verbose);
            });
        }
    }
}