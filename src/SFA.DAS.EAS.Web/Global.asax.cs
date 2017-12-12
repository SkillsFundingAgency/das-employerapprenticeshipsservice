using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FluentValidation.Mvc;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using NLog;
using NLog.Targets;
using SFA.DAS.Audit.Client;
using SFA.DAS.Audit.Client.Web;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Infrastructure.Logging;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.EAS.Web.Plumbing.Mvc;
using SFA.DAS.Web.Policy;

namespace SFA.DAS.EAS.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly RedisTarget RedisTarget; // Required to ensure assembly is copied to output.

        protected void Application_Start()
        {
            LoggingConfig.ConfigureLogging();

            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("InstrumentationKey");
            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(string), new TrimStringModelBinder());

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            FluentValidationModelValidatorProvider.Configure();

            WebMessageBuilders.Register();
            WebMessageBuilders.UserIdClaim = DasClaimTypes.Id;
            WebMessageBuilders.UserEmailClaim = DasClaimTypes.Email;

            AuditMessageFactory.RegisterBuilder(message =>
            {
                message.Source = new Source
                {
                    Component = "EAS-Web",
                    System = "EAS",
                    Version = typeof(MvcApplication).Assembly.GetName().Version.ToString()
                };
            });
        }
        
        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            var tc = new TelemetryClient();

            if (httpException != null && httpException.GetHttpCode() == (int)HttpStatusCode.NotFound)
            {
                return;
            }

            Logger.Error(exception, "Unhandled exception - " + exception.Message);
            tc.TrackTrace($"{exception.Message} - {exception.InnerException}");
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            new HttpContextPolicyProvider(
                new List<IHttpContextPolicy>()
                {
                    new ResponseHeaderRestrictionPolicy()
                }
            ).Apply(new HttpContextWrapper(HttpContext.Current), PolicyConcern.HttpResponse);
        }
    }
}
