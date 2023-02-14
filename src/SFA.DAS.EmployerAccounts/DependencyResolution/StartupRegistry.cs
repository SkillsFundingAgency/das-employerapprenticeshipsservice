using SFA.DAS.EmployerAccounts.Startup;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class StartupRegistry : Registry
{
    public StartupRegistry()
    {
        Scan(s =>
        {
            s.AssembliesAndExecutablesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
            s.Convention<CompositeDecorator<DefaultStartup, IStartup>>();
        });
    }
}