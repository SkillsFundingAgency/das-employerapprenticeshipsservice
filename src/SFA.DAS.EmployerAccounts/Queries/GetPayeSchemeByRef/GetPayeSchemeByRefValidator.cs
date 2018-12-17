﻿using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef
{
    public class GetPayeSchemeByRefValidator : IValidator<GetPayeSchemeByRefQuery>
    {
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