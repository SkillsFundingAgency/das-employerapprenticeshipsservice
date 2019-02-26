using System.Web;

namespace SFA.DAS.StuctureMap.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// This extension will prevent the "Failure while building 'Lambda: new HttpContextWrapper(HttpContext.Current)'" error that is raised when an AppDomain is being recycled.
        /// </summary>        
        public static HttpContextBase ToHttpContextBase(this HttpContext httpContext)
        {
            return httpContext == null ? new NullHttpContext() : new HttpContextWrapper(httpContext) as HttpContextBase;
        }
    }
}
