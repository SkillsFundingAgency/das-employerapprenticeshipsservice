//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Web;
//using System.Configuration;
//using System.Web.Http;
//using System.Web.Mvc;
//using System.Web.Routing;
//using Microsoft.ApplicationInsights.Extensibility;
//using SFA.DAS.NLog.Logger;
//using SFA.DAS.Web.Policy;
//using SFA.DAS.EAS.Support.Web;

//namespace SFA.DAS.EAS.Support.Web
//{
//    [ExcludeFromCodeCoverage]
//    public class Global : HttpApplication
//    {
//        private void Application_Start(object sender, EventArgs e)
//        {
//            MvcHandler.DisableMvcResponseHeader = true;
//            var logger = DependencyResolver.Current.GetService<ILog>();
//            logger.Info("Starting Web Role");

//            SetupApplicationInsights();

//            AreaRegistration.RegisterAllAreas();
//            GlobalConfiguration.Configure(WebApiConfig.Register);
//            RouteConfig.RegisterRoutes(RouteTable.Routes);

//            logger.Info("Web role started");
//        }

//        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
//        {
//            if (HttpContextHelper.Current == null) return;

//            new HttpContextPolicyProvider(
//                new List<IHttpContextPolicy>()
//                {
//                    new ResponseHeaderRestrictionPolicy()
//                }
//            ).Apply(new HttpContextWrapper(HttpContextHelper.Current), PolicyConcern.HttpResponse);
//        }
//        protected void Application_Error(object sender, EventArgs e)
//        {
//            var ex = Server.GetLastError().GetBaseException();
//            var logger = DependencyResolver.Current.GetService<ILog>();
//            logger.Error(ex, "App_Error");
//        }

//        private void SetupApplicationInsights()
//        {
//            TelemetryConfiguration.Active.InstrumentationKey =ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];

//            TelemetryConfiguration.Active.TelemetryInitializers.Add(new ApplicationInsightsInitializer());
//        }
//    }
//}