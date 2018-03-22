using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Validation;

namespace SFA.DAS.EAS.Web.Attributes
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