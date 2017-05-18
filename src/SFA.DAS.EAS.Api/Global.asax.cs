using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.EAS.Infrastructure.Logging;
using Microsoft.Azure;
using Microsoft.ApplicationInsights.Extensibility;

namespace SFA.DAS.EAS.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LoggingConfig.ConfigureLogging();

            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("InstrumentationKey");

            GlobalConfiguration.Configure(WebApiConfig.Register);


        }
    }
}
