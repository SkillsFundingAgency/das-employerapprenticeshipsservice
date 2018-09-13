using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;
using SFA.DAS.Tasks.API.Types.Enums;

namespace SFA.DAS.EAS.Application.Commands.DismissMonthlyTaskReminder
{
    public class DismissMonthlyTaskReminderCommandValidator : IValidator<DismissMonthlyTaskReminderCommand>
    {
        public ValidationResult Validate(DismissMonthlyTaskReminderCommand command)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(command.HashedAccountId))
            {
                validationResult.AddError(nameof(command.HashedAccountId), "Hashed account Id cannot be null or empty");
            }

            if (string.IsNullOrEmpty(command.ExternalUserId))
            {
                validationResult.AddError(nameof(command.ExternalUserId), "Hashed user Id cannot be null or empty");
            }

            if (command.TaskType == TaskType.None)
            {
                validationResult.AddError(nameof(command.TaskType), "Task type must be set");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(DismissMonthlyTaskReminderCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
