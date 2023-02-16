using SFA.DAS.EmployerAccounts.DependencyResolution;
using SFA.DAS.EmployerAccounts.ReadStore.DependencyResolution;
using SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution.StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;

public static class IoC
{
    public static void Initialize(Registry registry)
    {
        registry.IncludeRegistry<StartupRegistry>();
        registry.IncludeRegistry<CachesRegistry>();
        registry.IncludeRegistry<ConfigurationRegistry>();
        registry.IncludeRegistry<DataRegistry>();
        registry.IncludeRegistry<EventsRegistry>();
        registry.IncludeRegistry<ExecutionPoliciesRegistry>();
        registry.IncludeRegistry<LoggerRegistry>();
        registry.IncludeRegistry<MapperRegistry>();
        //registry.IncludeRegistry<MessagePublisherRegistry>();
        registry.IncludeRegistry<NotificationsRegistry>();
        registry.IncludeRegistry<NServiceBusUnitOfWorkRegistry>();
        registry.IncludeRegistry<RepositoriesRegistry>();
        registry.IncludeRegistry<ReadStoreDataRegistry>();
        registry.IncludeRegistry<DefaultRegistry>();
    }
}