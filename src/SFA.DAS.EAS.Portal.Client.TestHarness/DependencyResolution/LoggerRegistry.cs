using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EAS.Portal.Client.TestHarness.DependencyResolution
{
    public class LoggerRegistry : Registry
    {
        public LoggerRegistry()
        {
            For<ILog>().Use(c => new NLogLogger(c.ParentType, null, null)).AlwaysUnique();
        }
    }
}