namespace SFA.DAS.EmployerAccounts.Commands.UnsubscribeNotification;

public class UnsubscribeNotificationValidator : IValidator<UnsubscribeNotificationCommand>
{
    public ValidationResult Validate(UnsubscribeNotificationCommand item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.UserRef))
        {
            validationResult.AddError(nameof(item.UserRef), "User id cannot be null");
        }

        if (item.AccountId < 1)
        {
            validationResult.AddError(nameof(item.AccountId), "Account id must be more that 0");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(UnsubscribeNotificationCommand item)
    {
        return Task.FromResult(Validate(item));
    }
}