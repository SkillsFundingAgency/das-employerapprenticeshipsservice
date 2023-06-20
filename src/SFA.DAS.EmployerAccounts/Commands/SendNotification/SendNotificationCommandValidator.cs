namespace SFA.DAS.EmployerAccounts.Commands.SendNotification;

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
            
        if (string.IsNullOrEmpty(item.Email.TemplateId))
        {
            validationResult.AddError(nameof(item.Email.TemplateId), "TemplateId has not been supplied");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(SendNotificationCommand item)
    {
        return Task.FromResult(Validate(item));
    }
}