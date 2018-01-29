using System;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpNotFoundForNullModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var result = filterContext.Result as ViewResultBase;

            if (result != null && result.Model == null)
            {
                filterContext.Result = new HttpNotFoundResult();
            }
        }
    }
}