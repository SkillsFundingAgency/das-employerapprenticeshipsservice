using System;
using System.Linq;
using System.Web.Mvc;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var model = filterContext.ActionParameters.Values.OfType<ViewModel>().Single();
            var tempData = filterContext.Controller.TempData;
            var viewData = filterContext.Controller.ViewData;

            viewData.Model = model;

            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                filterContext.Result = new ViewResult
                {
                    ViewData = viewData,
                    TempData = tempData
                };
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var commandException = filterContext.Exception as CommandException;

            if (commandException != null)
            {
                var partialFieldName = ExpressionHelper.GetExpressionText(commandException.Expression);
                var fullHtmlFieldName = $"{nameof(ViewModel<object>.Message)}.{partialFieldName}";

                filterContext.Controller.ViewData.ModelState.AddModelError(fullHtmlFieldName, commandException.Message);

                var model = filterContext.Controller.ViewData.Model;
                var tempData = filterContext.Controller.TempData;
                var viewData = filterContext.Controller.ViewData;

                viewData.Model = model;
                
                filterContext.Result = new ViewResult
                {
                    ViewData = viewData,
                    TempData = tempData
                };

                filterContext.ExceptionHandled = true;
            }
        }
    }
}