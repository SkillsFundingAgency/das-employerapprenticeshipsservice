using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddModelError(this ModelStateDictionary modelState, ValidationException ex)
        {
            if (ex.MessageType != null && !string.IsNullOrWhiteSpace(ex.PropertyName))
            {
                modelState.AddModelError($"{ex.MessageType.Name}.{ex.PropertyName}", ex.Message);
            }
            else
            {
                modelState.AddModelError("", ex.Message);
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