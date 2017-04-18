using SFA.DAS.EAS.Application.Validation;
using System;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class ValidationResultExtensions
    {
        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            if (!result.IsValid())
            {
                foreach (var error in result.ValidationDictionary)
                {
                    modelState.AddModelError(error.Key, error.Value);
                }
            }
        }
    }
}