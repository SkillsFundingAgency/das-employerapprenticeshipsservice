﻿using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.DeleteInvitation
{
    public class DeleteInvitationCommandValidator : IValidator<DeleteInvitationCommand>
    {
        public ValidationResult Validate(DeleteInvitationCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.Email))
                validationResult.AddError("Email", "No Id supplied");

            if (string.IsNullOrEmpty(item.HashedAccountId))
                validationResult.AddError("HashedId", "No HashedId supplied");

            if (item.ExternalUserId.Equals(Guid.Empty))
                validationResult.AddError("ExternalUserId", "No ExternalUserId supplied");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(DeleteInvitationCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}