using System;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class ModelStateTempDataAttribute : ActionFilterAttribute
    {
        private static readonly string Key = typeof(ModelStateTempDataAttribute).FullName;
     
        protected void ExportModelStateToTempData(ControllerContext context)
        {
            context.Controller.TempData[Key] = context.Controller.ViewData.ModelState;
        }

        protected void ImportModelStateFromTempData(ControllerContext context)
        {
            context.Controller.ViewData.ModelState.Merge(context.Controller.TempData[Key] as ModelStateDictionary);
        }

        protected void RemoveModelStateFromTempData(ControllerContext context)
        {
            context.Controller.TempData[Key] = null;
        }
    }
}