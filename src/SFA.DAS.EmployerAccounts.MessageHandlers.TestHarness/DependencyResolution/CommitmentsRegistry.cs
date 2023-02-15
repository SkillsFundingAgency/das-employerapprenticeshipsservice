using StructureMap;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.DependencyResolution;

public class CommitmentsRegistry : Registry
{
    public CommitmentsRegistry()
    {
        Scan(s =>
        {
            s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.Commitments"));
            s.RegisterConcreteTypesAgainstTheFirstInterface();
        });
    }
}