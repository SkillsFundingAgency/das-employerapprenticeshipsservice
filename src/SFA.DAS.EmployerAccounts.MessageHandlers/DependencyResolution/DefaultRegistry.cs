namespace SFA.DAS.EmployerAccounts.MessageHandlers.DependencyResolution;

public class DefaultRegistry : Registry
{
    public DefaultRegistry()
    {
        Scan(s =>
        {
            //s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.Messaging"));
            s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.Employer"));
            s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.Notifications"));
            s.RegisterConcreteTypesAgainstTheFirstInterface();
        });
    }
}