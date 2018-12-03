﻿using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Application.Commands.UpdateOrganisationDetails
{
    public class UpdateOrganisationDetailsCommandValidator : IValidator<UpdateOrganisationDetailsCommand>
    {
        public ValidationResult Validate(UpdateOrganisationDetailsCommand item)
        {
            return new ValidationResult();
        }

        public Task<ValidationResult> ValidateAsync(UpdateOrganisationDetailsCommand item)
        {
            return Task.FromResult(Validate(item));
        }
    }
}
