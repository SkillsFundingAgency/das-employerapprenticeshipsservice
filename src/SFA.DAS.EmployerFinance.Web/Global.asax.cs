﻿using Microsoft.ApplicationInsights;
using NLog;
using SFA.DAS.EmployerFinance.Web.Logging;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using SFA.DAS.Logging;
using SFA.DAS.Web.Policy;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SFA.DAS.Configuration;
using Environment = SFA.DAS.Configuration.Environment;
using Microsoft.ApplicationInsights.Extensibility;
using System.Configuration;
using SFA.DAS.Audit.Client.Web;
using SFA.DAS.EmployerUsers.WebClientComponents;
using SFA.DAS.Audit.Client;
using SFA.DAS.Audit.Types;
using SFA.DAS.EmployerFinance.Startup;

namespace SFA.DAS.EmployerFinance.Web
{
    public class MvcApplication : HttpApplication
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            AntiForgeryConfig.RequireSsl = true;
            AreaRegistration.RegisterAllAreas();
            BinderConfig.RegisterBinders(ModelBinders.Binders);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            LoggingConfig.ConfigureLogging();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];
            WebMessageBuilders.Register();
            WebMessageBuilders.UserIdClaim = DasClaimTypes.Id;
            WebMessageBuilders.UserEmailClaim = DasClaimTypes.Email;

            AuditMessageFactory.RegisterBuilder(m =>
            {
                m.Source = new Source
                {
                    Component = "EmployerFinance-Web",
                    System = "EmployerFinance",
                    Version = typeof(MvcApplication).Assembly.GetName().Version.ToString()
                };
            });

            if (ConfigurationHelper.IsEnvironmentAnyOf(Environment.Local, Environment.At, Environment.Test))
            {
                SystemDetailsViewModel.EnvironmentName = ConfigurationHelper.CurrentEnvironment.ToString();
                SystemDetailsViewModel.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            DependencyResolver.Current.GetService<IStartup>().StartAsync().GetAwaiter().GetResult();
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            new HttpContextPolicyProvider(new List<IHttpContextPolicy> { new ResponseHeaderRestrictionPolicy() })
                .Apply(new HttpContextWrapper(HttpContext.Current), PolicyConcern.HttpResponse);
        }

        protected void Application_End()
        {
            DependencyResolver.Current.GetService<IStartup>().StopAsync().GetAwaiter().GetResult();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            if (exception is HttpException httpException && httpException.GetHttpCode() == (int)HttpStatusCode.NotFound)
            {
                return;
            }

            Dictionary<string, object> properties = null;

            try
            {
                properties = new Dictionary<string, object>
                {
                    ["HttpMethod"] = Request.HttpMethod,
                    ["IsAuthenticated"] = Request.IsAuthenticated,
                    ["Url"] = Request.Url.PathAndQuery,
                    ["UrlReferrer"] = Request.UrlReferrer?.PathAndQuery
                };
            }
            catch (Exception)
            {
                // Request not available
            }

            var message = exception.GetMessage();
            var telemetryClient = new TelemetryClient();

            Logger.Error(exception, message, properties);
            telemetryClient.TrackException(exception);
        }
    }
}
