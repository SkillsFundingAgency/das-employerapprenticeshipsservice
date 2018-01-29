using System;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateModelStateAttribute : ModelStateTempDataAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                ExportModelStateToTempData(filterContext);

                foreach (string key in filterContext.HttpContext.Request.QueryString.Keys)
                {
                    if (!filterContext.RouteData.Values.ContainsKey(key))
                    {
                        filterContext.RouteData.Values.Add(key, filterContext.HttpContext.Request.QueryString[key]);
                    }
                }

                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.Controller.ViewData.ModelState.IsValid && (filterContext.Result is RedirectResult || filterContext.Result is RedirectToRouteResult))
            {
                ExportModelStateToTempData(filterContext);
            }
        }
    }
}