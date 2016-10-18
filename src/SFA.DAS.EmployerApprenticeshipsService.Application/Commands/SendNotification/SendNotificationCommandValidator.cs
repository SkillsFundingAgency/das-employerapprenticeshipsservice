using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification
{
    public class SendNotificationCommandValidator : IValidator<SendNotificationCommand>
    {
        public ValidationResult Validate(SendNotificationCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.Email == null)
            {
                validationResult.AddError(nameof(item.Email), "Email has not been defined");
                return validationResult;
            }

            if (string.IsNullOrEmpty(item.Email.RecipientsAddress))
            {
                validationResult.AddError(nameof(item.Email.RecipientsAddress), "RecipientsAddress has not been supplied");
            }

            if (string.IsNullOrEmpty(item.Email.ReplyToAddress))
            {
                validationResult.AddError(nameof(item.Email.ReplyToAddress), "ReplyToAddress has not been supplied");
            }

            if (string.IsNullOrEmpty(item.Email.Subject))
            {
                validationResult.AddError(nameof(item.Email.Subject), "Subject has not been supplied");
            }

            if (string.IsNullOrEmpty(item.Email.SystemId))
            {
                validationResult.AddError(nameof(item.Email.SystemId), "SystemId has not been supplied");
            }

            if (string.IsNullOrEmpty(item.Email.TemplateId))
            {
                validationResult.AddError(nameof(item.Email.TemplateId), "TemplateId has not been supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(SendNotificationCommand item)
        {
            throw new NotImplementedException();
        }
    }
}