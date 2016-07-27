using System;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification
{
    public class SendNotificationCommandValidator : IValidator<SendNotificationCommand>
    {
        public ValidationResult Validate(SendNotificationCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.UserId == 0)
            {
                validationResult.AddError(nameof(item.UserId),"User Id has not been supplied");
            }

            if (item.Data == null)
            {
                validationResult.AddError(nameof(item.Data), "EmailContent has not been supplied");
            }
            else
            {
                if (string.IsNullOrEmpty(item.Data.RecipientsAddress))
                {
                    validationResult.AddError(nameof(item.Data.RecipientsAddress), "Recipients Address has not been supplied");
                }
            }

            return validationResult;
        }
    }
}