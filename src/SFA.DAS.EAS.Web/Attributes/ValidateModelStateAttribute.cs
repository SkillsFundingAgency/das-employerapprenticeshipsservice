using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Web.Extensions;

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

                filterContext.Result = GetRedirectToRouteResult(filterContext);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var validationException = filterContext.Exception as ValidationException;

            if (validationException != null)
            {
                filterContext.Controller.ViewData.ModelState.AddModelError(validationException);

                ExportModelStateToTempData(filterContext);

                filterContext.Result = GetRedirectToRouteResult(filterContext);
                filterContext.ExceptionHandled = true;
            }
        }

        private RedirectToRouteResult GetRedirectToRouteResult(ControllerContext context)
        {
            foreach (string key in context.HttpContext.Request.QueryString.Keys)
            {
                if (!context.RouteData.Values.ContainsKey(key))
                {
                    context.RouteData.Values.Add(key, context.HttpContext.Request.QueryString[key]);
                }
            }

            return new RedirectToRouteResult(context.RouteData.Values);
        }
    }
}