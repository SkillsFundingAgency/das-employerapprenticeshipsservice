﻿using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetOrganisationById
{
    public class GetOrganisationByIdValidator : IValidator<GetOrganisationByIdRequest>
    {
        public ValidationResult Validate(GetOrganisationByIdRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.Identifier))
            {
                validationResult.AddError(nameof(item.Identifier));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetOrganisationByIdRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}