using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class DateTimeRegistry : Registry
    {
        public DateTimeRegistry()
        {
            Policies.Add<CurrentDatePolicy>();
        }
    }
}