﻿using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationsByAorn
{
    public class GetOrganisationsByAornValidator : IValidator<GetOrganisationsByAornRequest>
    {
        public ValidationResult Validate(GetOrganisationsByAornRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.Aorn))
            {
                validationResult.AddError(nameof(item.Aorn));
            }

            if (string.IsNullOrEmpty(item.PayeRef))
            {
                validationResult.AddError(nameof(item.PayeRef));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetOrganisationsByAornRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}