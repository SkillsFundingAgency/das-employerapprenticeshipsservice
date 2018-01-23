using System.Web;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Api.Logging
{
    public sealed class RequestContext : IRequestContext
    {
        public string IpAddress { get; }
        public string Url { get; }
        
        public RequestContext(HttpContextBase context)
        {
            IpAddress = context?.Request.UserHostAddress;
            Url = context?.Request.RawUrl;
        }
    }
}