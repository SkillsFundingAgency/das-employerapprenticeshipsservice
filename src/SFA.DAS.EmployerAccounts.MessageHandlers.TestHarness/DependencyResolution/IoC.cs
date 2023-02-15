using SFA.DAS.EmployerAccounts.DependencyResolution;
using SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution.StructureMap;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.DependencyResolution;

public static class IoC
{
    public static IContainer Initialize()
    {
        return new Container(c =>
        {
            c.AddRegistry<ConfigurationRegistry>();
            c.AddRegistry<DataRegistry>();
            c.AddRegistry<StartupRegistry>();
            c.AddRegistry<NServiceBusUnitOfWorkRegistry>();
            c.AddRegistry<EventsRegistry>();
            c.AddRegistry<ExecutionPoliciesRegistry>();
            c.AddRegistry<MediatorRegistry>();
            c.AddRegistry<MessagePublisherRegistry>();
            c.AddRegistry<NotificationsRegistry>();
            c.AddRegistry<RepositoriesRegistry>();
            c.AddRegistry<CommitmentsRegistry>();
        });
    }
}
