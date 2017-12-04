using System.Web.Mvc;
using Microsoft.ApplicationInsights;
using NLog;

namespace SFA.DAS.EAS.Web.Plumbing.Mvc
{
    public class LogAndHandleErrorAttribute : HandleErrorAttribute
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public override void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception;
            var tc = new TelemetryClient();

            Logger.Error(exception, "Unhandled exception - " + exception.Message);
            tc.TrackException(exception);

            base.OnException(filterContext);
        }
    }
}