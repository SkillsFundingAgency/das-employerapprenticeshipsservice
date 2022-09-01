using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SFA.DAS.NLog.Logger;
using StructureMap;
using System.Web;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class LoggerRegistry : Registry
    {
        public LoggerRegistry()
        {
            For<ILog>().Use(c => new NLogLogger(c.ParentType, c.GetInstance<ILoggingContext>(), null)).AlwaysUnique();
            For<ILoggerFactory>().Use(() => new LoggerFactory().AddNLog()).Singleton();
            For<ILoggingContext>().Use(c => HttpContext.Current == null ? null : new LoggingContext(new HttpContextWrapper(HttpContext.Current)));
        }
    }

    public sealed class LoggingContext : ILoggingContext
    {
        public string HttpMethod { get; set; }
        public bool? IsAuthenticated { get; set; }
        public string Url { get; }
        public string UrlReferrer { get; set; }
        public string ServerMachineName { get; set; }

        public LoggingContext(HttpContextBase context)
        {
            HttpMethod = context?.Request.HttpMethod;
            IsAuthenticated = context?.Request.IsAuthenticated;
            Url = context?.Request.Url?.PathAndQuery;
            UrlReferrer = context?.Request.UrlReferrer?.PathAndQuery;
            ServerMachineName = context?.Server.MachineName;
        }
    }
}