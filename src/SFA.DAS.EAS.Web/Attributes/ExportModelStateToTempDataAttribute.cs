using System;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExportModelStateToTempDataAttribute : ModelStateTempDataAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.Controller.ViewData.ModelState.IsValid && (filterContext.Result is RedirectResult || filterContext.Result is RedirectToRouteResult))
            {
                ExportModelStateToTempData(filterContext);
            }
        }
    }
}