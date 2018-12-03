﻿using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetCharity
{
    public class GetCharityQueryValidator : IValidator<GetCharityQueryRequest>
    {
        public ValidationResult Validate(GetCharityQueryRequest item)
        {
            var validationResult = new ValidationResult();

            if (item.RegistrationNumber == 0)
            {
                validationResult.AddError(nameof(item.RegistrationNumber));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetCharityQueryRequest item)
        {
            throw new NotImplementedException();
        }
    }
}
