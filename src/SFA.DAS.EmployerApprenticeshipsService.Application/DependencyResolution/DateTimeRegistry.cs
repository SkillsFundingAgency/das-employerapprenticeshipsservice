using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class DateTimeRegistry : Registry
    {
        public DateTimeRegistry()
        {
            Policies.Add<CurrentDatePolicy>();
        }
    }
}