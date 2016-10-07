using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;


namespace SFA.DAS.EAS.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();
        }
    }
}
