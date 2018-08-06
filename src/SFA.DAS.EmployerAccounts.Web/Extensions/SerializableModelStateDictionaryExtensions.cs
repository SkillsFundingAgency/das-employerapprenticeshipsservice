﻿using SFA.DAS.EmployerAccounts.Web.Validation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Extensions
{
    public static class SerializableModelStateDictionaryExtensions
    {
        public static ModelStateDictionary ToModelState(this SerializableModelStateDictionary serializableModelState)
        {
            var modelState = new ModelStateDictionary();

            foreach (var data in serializableModelState.Data)
            {
                ValueProviderResult value = null;

                if (data.RawValue != null || data.AttemptedValue != null || data.CultureName != null)
                {
                    value = new ValueProviderResult(data.RawValue, data.AttemptedValue, data.CultureName == null ? null : CultureInfo.GetCultureInfo(data.CultureName));
                }

                modelState.SetModelValue(data.Key, value);

                foreach (var error in data.ErrorMessages)
                {
                    modelState.AddModelError(data.Key, error);
                }
            }

            return modelState;
        }
    }
}
