using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Validation;
using System;
using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Web.Extensions;

namespace SFA.DAS.EmployerAccounts.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ImportModelStateFromTempDataAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var serializeableModelState = filterContext.Controller.TempData[ControllerConstants.ModelStateTempDataKey] as SerializableModelStateDictionary;
            var modelState = serializeableModelState?.ToModelState();

            filterContext.Controller.ViewData.ModelState.Merge(modelState);
        }
    }
}