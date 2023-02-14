using SFA.DAS.EmployerAccounts.DependencyResolution;
using SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution.StructureMap;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;

public static class IoC
{
    public static void Initialize(Registry registry)
    {
        registry.IncludeRegistry<ConfigurationRegistry>();
        registry.IncludeRegistry<DataRegistry>();
        registry.IncludeRegistry<StartupRegistry>();
        registry.IncludeRegistry<NServiceBusUnitOfWorkRegistry>();
    }
}