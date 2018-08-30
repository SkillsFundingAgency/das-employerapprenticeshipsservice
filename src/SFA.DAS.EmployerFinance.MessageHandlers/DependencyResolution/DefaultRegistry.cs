using StructureMap;

namespace SFA.DAS.EmployerFinance.MessageHandlers.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS."));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });
        }
    }
}