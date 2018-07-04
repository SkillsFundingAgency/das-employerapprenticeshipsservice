using SFA.DAS.NLog.Logger;
using System.Web;

namespace SFA.DAS.EAS.Application.Logging
{
    public sealed class LoggingContext : ILoggingContext
    {
        public string HttpMethod { get; set; }
        public bool? IsAuthenticated { get; set; }
        public string Url { get; }
        public string UrlReferrer { get; set; }
        public string ServerMachineName { get; set; }

        public LoggingContext(HttpContextBase context)
        {
            HttpMethod = context?.Request.HttpMethod;
            IsAuthenticated = context?.Request.IsAuthenticated;
            Url = context?.Request.Url?.PathAndQuery;
            UrlReferrer = context?.Request.UrlReferrer?.PathAndQuery;
            ServerMachineName = context?.Server.MachineName;
        }
    }
}