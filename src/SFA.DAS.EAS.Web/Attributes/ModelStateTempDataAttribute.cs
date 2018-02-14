using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.Serialization;

namespace SFA.DAS.EAS.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class ModelStateTempDataAttribute : ActionFilterAttribute
    {
        private static readonly string Key = typeof(ModelStateTempDataAttribute).FullName;
     
        protected void ExportModelStateToTempData(ControllerContext context)
        {
            var serializeableModelState = context.Controller.ViewData.ModelState.ToSerializable();

            context.Controller.TempData[Key] = serializeableModelState;
        }

        protected void ImportModelStateFromTempData(ControllerContext context)
        {
            var serializeableModelState = context.Controller.TempData[Key] as SerializableModelStateDictionary;
            var modelState = serializeableModelState?.ToModelState();

            context.Controller.ViewData.ModelState.Merge(modelState);
        }

        protected void RemoveModelStateFromTempData(ControllerContext context)
        {
            context.Controller.TempData[Key] = null;
        }
    }
}