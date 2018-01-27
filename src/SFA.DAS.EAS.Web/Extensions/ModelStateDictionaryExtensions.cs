using System.Web.Mvc;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddValidationResult(this ModelStateDictionary modelState, ValidationResult validationResult)
        {
            foreach (var error in validationResult.ValidationDictionary)
            {
                modelState.AddModelError($"{nameof(ViewModel<object>.Message)}.{error.Key}", error.Value);
            }
        }
    }
}