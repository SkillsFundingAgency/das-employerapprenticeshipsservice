using SFA.DAS.EAS.Portal.Client.TestHarness.Scenarios;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.TestHarness.DependencyResolution
{
    public class ScenariosRegistry : Registry
    {
        public ScenariosRegistry()
        {
            For<GetAccountScenario>().Use<GetAccountScenario>();
        }
    }
}