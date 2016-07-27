using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ProcessNotification
{
    public class ProcessNotificationCommandValidator : IValidator<ProcessNotificationCommand>
    {
        public ValidationResult Validate(ProcessNotificationCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.Id == 0)
            {
                validationResult.AddError(nameof(item.Id),"No value supplied for the Notificaiton Command Id");
            }

            return validationResult;
        }
    }
}