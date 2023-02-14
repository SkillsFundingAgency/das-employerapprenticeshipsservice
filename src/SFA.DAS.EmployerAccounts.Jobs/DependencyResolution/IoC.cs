using SFA.DAS.EmployerAccounts.DependencyResolution;
using SFA.DAS.EmployerAccounts.ReadStore.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Jobs.DependencyResolution;

public static class IoC
{
    public static void Initialize(Registry registry)
    {
        registry.IncludeRegistry<ConfigurationRegistry>();
        registry.IncludeRegistry<DataRegistry>();
        registry.IncludeRegistry<ReadStoreDataRegistry>();
    }
}