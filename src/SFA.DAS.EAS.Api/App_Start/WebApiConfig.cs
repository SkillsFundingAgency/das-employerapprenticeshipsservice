using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Azure;
using System.Net.Http.Headers;
using System.Web.Http;
using SFA.DAS.ApiTokens.Client;
using System.Web.Http.ExceptionHandling;

namespace SFA.DAS.EAS.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            config.MapHttpAttributeRoutes();

            config.Services.Replace(typeof(IExceptionHandler), new CustomExceptionHandler());
        }
    }
}
