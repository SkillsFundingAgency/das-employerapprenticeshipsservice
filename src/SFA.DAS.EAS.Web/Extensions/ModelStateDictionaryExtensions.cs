using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Extensions;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddModelError(this ModelStateDictionary modelState, ValidationException ex)
        {
            modelState.AddModelError($"{nameof(ViewModel<object>.Message)}.{ex.PropertyName}", ex.ErrorMessage);
        }

        public static ModelStateDictionary DistinctByErrorMessage(this ModelStateDictionary modelState)
        {
            var newModelState2 = new ModelStateDictionary();
            newModelState2.AddValidationResult(new ValidationResult
            {
                ValidationDictionary =
                    modelState.Where(kvp => kvp.Value.Errors.Any())
                        .DistinctBy(s => s.Value.Errors.First().ErrorMessage)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors[0].ErrorMessage)
            });

            return newModelState2;
        }

    }
}