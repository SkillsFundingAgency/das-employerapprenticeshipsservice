using System;
using System.Threading.Tasks;

using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.UnsubscribeNotification
{
    public class UnsubscribeNotificationValidator : IValidator<UnsubscribeNotificationCommand>
    {
        public ValidationResult Validate(UnsubscribeNotificationCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.ExternalUserId.Equals(Guid.Empty))
            {
                validationResult.AddError(nameof(item.ExternalUserId), "External User Id cannot be null");
            }

            if (item.AccountId < 1)
            {
                validationResult.AddError(nameof(item.AccountId), "Account id must be more that 0");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(UnsubscribeNotificationCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}