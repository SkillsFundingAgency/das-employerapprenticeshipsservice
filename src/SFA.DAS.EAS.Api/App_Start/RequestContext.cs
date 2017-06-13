using SFA.DAS.NLog.Logger;
using System.Web;

namespace SFA.DAS.EAS.Api.App_Start
{
    public sealed class RequestContext : IRequestContext
    {
        public RequestContext(HttpContextBase context)
        {
            IpAddress = context?.Request.UserHostAddress;
            Url = context?.Request.RawUrl;
        }

        public string IpAddress { get; private set; }

        public string Url { get; private set; }
    }
}