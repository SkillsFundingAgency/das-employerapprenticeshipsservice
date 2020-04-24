﻿using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserByEmail
{
    public class GetUserByEmailQueryValidator : IValidator<GetUserByEmailQuery>
    {
        public ValidationResult Validate(GetUserByEmailQuery item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.Email))
            {
                result.AddError(nameof(item.Email), "User email has not been supplied");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(GetUserByEmailQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
