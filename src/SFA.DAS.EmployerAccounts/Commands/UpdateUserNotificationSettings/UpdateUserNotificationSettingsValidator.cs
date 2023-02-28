namespace SFA.DAS.EmployerAccounts.Commands.UpdateUserNotificationSettings;

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
        return Task.FromResult(Validate(item));
    }
}