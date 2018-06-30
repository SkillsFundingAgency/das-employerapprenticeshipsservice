using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.EntityFramework;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class NServiceBusRegistry : Registry
    {
        public NServiceBusRegistry()
        {
            For<IEventPublisher>().Use<EventPublisher>();
            For<IOutboxDbContext>().Use(c => c.GetInstance<EmployerAccountDbContext>());
            For<IProcessOutboxMessagesJob>().Use<ProcessOutboxMessagesJob>();
            For<IUnitOfWorkContext>().Use<UnitOfWorkContext>();
            For<IUnitOfWorkManager>().Use<UnitOfWorkManager>();
        }
    }
}