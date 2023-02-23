namespace SFA.DAS.EmployerAccounts.Commands.UpdateShowWizard;

public class UpdateShowAccountWizardCommandValidator : IValidator<UpdateShowAccountWizardCommand>
{
    public ValidationResult Validate(UpdateShowAccountWizardCommand item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrWhiteSpace(item.HashedAccountId))
            validationResult.AddError(nameof(item.HashedAccountId));

        if (string.IsNullOrWhiteSpace(item.ExternalUserId))
            validationResult.AddError(nameof(item.ExternalUserId));

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(UpdateShowAccountWizardCommand item)
    {
        return Task.FromResult(Validate(item));
    }
}