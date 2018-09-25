﻿using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Commands.AcceptInvitation
{
    public class AcceptInvitationCommandValidator : IValidator<AcceptInvitationCommand>
    {
        public ValidationResult Validate(AcceptInvitationCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.Id == 0)
                validationResult.AddError("Id", "No Id supplied");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(AcceptInvitationCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}