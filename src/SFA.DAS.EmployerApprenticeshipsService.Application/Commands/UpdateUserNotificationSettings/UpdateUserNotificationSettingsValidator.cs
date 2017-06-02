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
                validationResult.AddError("UserRef", "No UserRef supplied");

            if (item.AccountId < 1)
                validationResult.AddError("AccountId", "No AccountId supplied");

            if (item.Settings == null)
                validationResult.AddError("Settings", "No Settings supplied");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(UpdateUserNotificationSettingsCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
