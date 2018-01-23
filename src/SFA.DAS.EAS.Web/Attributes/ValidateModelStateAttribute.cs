using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain;

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
                
                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var exception = filterContext.Exception as DomainException;

            if (exception != null)
            {
                var partialFieldName = ExpressionHelper.GetExpressionText(exception.Expression);
                var fullHtmlFieldName = $"{nameof(ViewModel<object>.Message)}.{partialFieldName}";

                filterContext.Controller.ViewData.ModelState.AddModelError(fullHtmlFieldName, exception.Message);

                ExportModelStateToTempData(filterContext);

                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
                filterContext.ExceptionHandled = true;
            }
        }
    }
}