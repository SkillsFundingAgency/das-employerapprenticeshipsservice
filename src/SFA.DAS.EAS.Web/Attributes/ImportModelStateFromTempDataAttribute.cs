using System;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ImportModelStateFromTempDataAttribute : ModelStateTempDataAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is ViewResult)
            {
                ImportModelStateFromTempData(filterContext);
            }
            else
            {
                RemoveModelStateFromTempData(filterContext);
            }
        }
    }
}