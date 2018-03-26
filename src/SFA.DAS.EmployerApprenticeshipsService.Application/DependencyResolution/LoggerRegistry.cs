using System.Web;
using SFA.DAS.EAS.Application.Logging;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EAS.Application.DependencyResolution
{
    public class LoggerRegistry : Registry
    {
        public LoggerRegistry()
        {
            For<ILog>().Use(c => new NLogLogger(c.ParentType, c.GetInstance<ILoggingContext>(), null)).AlwaysUnique();
            For<ILoggingContext>().Use(c => HttpContext.Current == null ? null : new LoggingContext(new HttpContextWrapper(HttpContext.Current)));
        }
    }
}