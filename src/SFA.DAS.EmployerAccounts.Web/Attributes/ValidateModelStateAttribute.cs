using SFA.DAS.EmployerAccounts.Web.Helpers;
using System;
using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.Exceptions;

namespace SFA.DAS.EmployerAccounts.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                filterContext.Controller.TempData[ControllerConstants.ModelStateTempDataKey] = filterContext.Controller.ViewData.ModelState.ToSerializable();
                filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.QueryString);
                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception is ValidationException validationException)
            {
                filterContext.Controller.ViewData.ModelState.AddModelError(validationException);
                filterContext.Controller.TempData[ControllerConstants.ModelStateTempDataKey] = filterContext.Controller.ViewData.ModelState.ToSerializable();
                filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.QueryString);
                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
                filterContext.ExceptionHandled = true;
            }
        }
    }
}