using System.Web.Mvc;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddModelError(this ModelStateDictionary modelState, ValidationException ex)
        {
            modelState.AddModelError($"{nameof(ViewModel<object>.Message)}.{ex.PropertyName}", ex.ErrorMessage);
        }
    }
}