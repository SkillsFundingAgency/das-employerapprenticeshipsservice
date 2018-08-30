using Microsoft.ApplicationInsights;
using NLog;
using System.Collections.Generic;
using System.Web.Http.ExceptionHandling;
using SFA.DAS.Logging;

namespace SFA.DAS.EAS.Account.Api.ExceptionLoggers
{
    public class ErrorLogger : ExceptionLogger
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public override void Log(ExceptionLoggerContext context)
        {
            var properties = new Dictionary<string, object>
            {
                ["HttpMethod"] = context.Request.Method,
                ["IsAuthenticated"] = context.RequestContext.Principal?.Identity?.IsAuthenticated ?? false,
                ["Url"] = context.Request.RequestUri.PathAndQuery,
                ["UrlReferrer"] = context.Request.Headers.Referrer?.PathAndQuery
            };

            var message = context.Exception.GetMessage();
            var telemetryClient = new TelemetryClient();

            Logger.Error(context.Exception, message, properties);
            telemetryClient.TrackException(context.Exception);
        }
    }
}