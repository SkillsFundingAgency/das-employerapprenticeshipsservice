using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.Logging
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
            HttpMethod = context?.Request.Method;
            IsAuthenticated = context?.Request.HttpContext.User.Identity?.IsAuthenticated;
            Url = context?.Request.GetEncodedPathAndQuery();
            UrlReferrer = context?.Request.GetTypedHeaders().Referer?.PathAndQuery;
            ServerMachineName = string.Empty; //context?.Server.MachineName; // TODO Fix this, although not sure if applicable.
        }
    }
}