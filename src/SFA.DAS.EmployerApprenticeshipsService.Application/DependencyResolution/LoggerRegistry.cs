using SFA.DAS.EAS.Application.Logging;
using SFA.DAS.NLog.Logger;
using StructureMap;
using SFA.DAS.EAS.Application;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class LoggerRegistry : Registry
    {
        public LoggerRegistry()
        {
            For<ILog>().Use(c => new NLogLogger(c.ParentType, c.GetInstance<ILoggingContext>(), null)).AlwaysUnique();
            For<ILoggingContext>().Use(c => HttpContextHelper.Current == null ? null : new LoggingContext(new HttpContextWrapper(HttpContextHelper.Current)));
        }
    }
}