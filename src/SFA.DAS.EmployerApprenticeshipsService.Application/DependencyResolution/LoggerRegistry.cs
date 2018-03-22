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
            For<ILog>().Use(c => new NLogLogger(c.ParentType, c.GetInstance<IRequestContext>(), null)).AlwaysUnique();
            For<IRequestContext>().Use(c => new RequestContext(new HttpContextWrapper(HttpContext.Current)));
        }
    }
}