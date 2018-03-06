using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace SFA.DAS.EAS.Account.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpNotFoundForNullModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (!actionExecutedContext.Response.TryGetContentValue(out object result) || result == null)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }
    }
}