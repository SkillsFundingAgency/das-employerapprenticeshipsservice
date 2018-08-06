using System;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpNotFoundForNullModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is ViewResultBase result && result.Model == null)
            {
                filterContext.Result = new HttpNotFoundResult();
            }
        }
    }
}