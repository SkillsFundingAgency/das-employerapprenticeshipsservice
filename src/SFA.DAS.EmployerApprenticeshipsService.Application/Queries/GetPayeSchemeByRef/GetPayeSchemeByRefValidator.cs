﻿using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef
{
    public class GetPayeSchemeByRefValidator : IValidator<GetPayeSchemeByRefQuery>
    {
        /// <summary>
        ///  AML-2454: Copy to finance
        /// </summary>
        public ValidationResult Validate(GetPayeSchemeByRefQuery item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.Ref), "HashedAccountId has not been supplied");
            }

            if (string.IsNullOrEmpty(item.Ref))
            {
                validationResult.AddError(nameof(item.Ref), "PayeSchemeRef has not been supplied");
            }
            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetPayeSchemeByRefQuery item)
        {
            throw new NotImplementedException();
        }
    }
}