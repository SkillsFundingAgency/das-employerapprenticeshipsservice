using System.Linq;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Web.Serialization;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddModelError(this ModelStateDictionary modelState, ValidationException ex)
        {
            if (string.IsNullOrEmpty(ex.PropertyName))
            {
                modelState.AddModelError("", ex.ErrorMessage);
            }
            else
            {
                modelState.AddModelError($"{nameof(ViewModel<object>.Message)}.{ex.PropertyName}", ex.ErrorMessage);
            }
        }

        public static SerializableModelStateDictionary ToSerializable(this ModelStateDictionary modelState)
        {
            var data = modelState
                .Select(kvp => new SerializableModelState
                {
                    AttemptedValue = kvp.Value.Value?.AttemptedValue,
                    CultureName = kvp.Value.Value?.Culture.Name,
                    ErrorMessages = kvp.Value.Errors.Select(e => e.ErrorMessage).ToList(),
                    Key = kvp.Key,
                    RawValue = kvp.Value.Value?.RawValue
                })
                .ToList();

            return new SerializableModelStateDictionary
            {
                Data = data
            };
        }
    }
}