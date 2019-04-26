using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Time;
using StructureMap;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class DateTimeRegistry : Registry
    {
        public DateTimeRegistry()
        {
            For<ICurrentDateTime>().Use<CurrentDateTime>().Singleton();
        }
    }
}