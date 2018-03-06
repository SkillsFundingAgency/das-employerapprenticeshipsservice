using System.Web.Http.ModelBinding;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Account.Api.Extensions
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
    }
}