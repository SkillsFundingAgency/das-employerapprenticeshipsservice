﻿using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.AddPayeWithExistingLegalEntity
{
    public class AddPayeToAccountForExistingLegalEntityCommandValidator : IValidator<AddPayeToAccountForExistingLegalEntityCommand>
    {
        public ValidationResult Validate(AddPayeToAccountForExistingLegalEntityCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedId))
                validationResult.AddError(nameof(item.HashedId), "No HashedId supplied");

            if (item.LegalEntityId == 0)
                validationResult.AddError(nameof(item.LegalEntityId), "No LegalEntityId supplied");

            if (string.IsNullOrWhiteSpace(item.EmpRef))
                validationResult.AddError(nameof(item.EmpRef), "No EmpRef supplied");

            if (string.IsNullOrWhiteSpace(item.ExternalUserId))
                validationResult.AddError(nameof(item.ExternalUserId), "No ExternalUserId supplied");

            if (string.IsNullOrWhiteSpace(item.AccessToken))
            {
                validationResult.AddError(nameof(item.AccessToken),"Access token has not been supplied");
            }

            if (string.IsNullOrWhiteSpace(item.RefreshToken))
            {
                validationResult.AddError(nameof(item.RefreshToken), "Refresh token has not been supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(AddPayeToAccountForExistingLegalEntityCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}