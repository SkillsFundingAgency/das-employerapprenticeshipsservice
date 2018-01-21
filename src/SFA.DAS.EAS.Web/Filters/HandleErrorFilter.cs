using System.Web.Mvc;
using Microsoft.ApplicationInsights;
using NLog;

namespace SFA.DAS.EAS.Web.Filters
{
    public class HandleErrorFilter : HandleErrorAttribute
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                var exception = filterContext.Exception;
                var telemetryClient = new TelemetryClient();

                Logger.Error(exception);
                telemetryClient.TrackException(exception);

                base.OnException(filterContext);
            }
        }
    }
}