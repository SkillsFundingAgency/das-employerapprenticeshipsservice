using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Application.Logging
{
    public sealed class LoggingContext : ILoggingContext
    {
        public string HttpMethod { get; set; }
        public bool? IsAuthenticated { get; set; }
        public string Url { get; }
        public string UrlReferrer { get; set; }
        public string ServerMachineName { get; set; }

        public LoggingContext(HttpContext context)
        {
            HttpMethod = context.Request.Method;
            IsAuthenticated = context.User.Identity.IsAuthenticated;
            Url = context.Request.Path + context.Request.QueryString;
            var header = context.Request.GetTypedHeaders();
            UrlReferrer = header.Referer.PathAndQuery;
            ServerMachineName = System.Environment.MachineName;
        }
    }
}