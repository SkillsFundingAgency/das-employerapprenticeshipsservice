using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.UpdateUserNotificationSettings
{
    public class UpdateUserNotificationSettingsValidator : IValidator<UpdateUserNotificationSettingsCommand>
    {
        public ValidationResult Validate(UpdateUserNotificationSettingsCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.ExternalUserId.Equals(Guid.Empty))
                validationResult.AddError(nameof(item.ExternalUserId));

            if (item.Settings == null)
                validationResult.AddError(nameof(item.Settings));

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(UpdateUserNotificationSettingsCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
