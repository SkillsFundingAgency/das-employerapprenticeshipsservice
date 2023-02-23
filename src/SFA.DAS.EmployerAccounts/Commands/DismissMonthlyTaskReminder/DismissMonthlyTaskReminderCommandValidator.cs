using SFA.DAS.EmployerAccounts.TasksApi;

namespace SFA.DAS.EmployerAccounts.Commands.DismissMonthlyTaskReminder;

public class DismissMonthlyTaskReminderCommandValidator : IValidator<DismissMonthlyTaskReminderCommand>
{
    public ValidationResult Validate(DismissMonthlyTaskReminderCommand item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.HashedAccountId))
        {
            validationResult.AddError(nameof(item.HashedAccountId), "Hashed account Id cannot be null or empty");
        }

        if (string.IsNullOrEmpty(item.ExternalUserId))
        {
            validationResult.AddError(nameof(item.ExternalUserId), "Hashed user Id cannot be null or empty");
        }

        if (item.TaskType == TaskType.None)
        {
            validationResult.AddError(nameof(item.TaskType), "Task type must be set");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(DismissMonthlyTaskReminderCommand item)
    {
        return Task.FromResult(Validate(item));
    }
}