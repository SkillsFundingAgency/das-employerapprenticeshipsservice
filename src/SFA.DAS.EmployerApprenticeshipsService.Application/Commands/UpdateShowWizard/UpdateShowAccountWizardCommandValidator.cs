using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.UpdateShowWizard
{
    public class UpdateShowAccountWizardCommandValidator : IValidator<UpdateShowAccountWizardCommand>
    {
        public ValidationResult Validate(UpdateShowAccountWizardCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.HashedAccountId))
                validationResult.AddError(nameof(item.HashedAccountId));

            if (item.ExternalUserId.Equals(Guid.Empty))
                validationResult.AddError(nameof(item.ExternalUserId));

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(UpdateShowAccountWizardCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
