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

            if (string.IsNullOrWhiteSpace(item.UserRef))
                validationResult.AddError(nameof(item.UserRef));

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
