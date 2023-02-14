using SFA.DAS.EmployerAccounts.DependencyResolution;
using SFA.DAS.EmployerAccounts.ReadStore.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EmployerAccounts.Jobs.DependencyResolution;

public static class IoC
{
    public static IContainer Initialize()
    {
        var container = new Container(c =>
        {
            c.AddRegistry<ConfigurationRegistry>();
            c.AddRegistry<DataRegistry>();
            c.AddRegistry<ReadStoreDataRegistry>();
            c.AddRegistry<LoggerRegistry>();
            c.AddRegistry<DefaultRegistry>();
        });

        ServiceLocator.Initialize(container);
        return container;
    }
}