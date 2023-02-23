namespace SFA.DAS.EmployerAccounts.Commands.DeleteInvitation;

public class DeleteInvitationCommandValidator : IValidator<DeleteInvitationCommand>
{
    public ValidationResult Validate(DeleteInvitationCommand item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrWhiteSpace(item.Email))
            validationResult.AddError("Email", "No Id supplied");

        if (string.IsNullOrEmpty(item.HashedAccountId))
            validationResult.AddError("HashedId", "No HashedId supplied");

        if (string.IsNullOrWhiteSpace(item.ExternalUserId))
            validationResult.AddError("ExternalUserId", "No ExternalUserId supplied");

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(DeleteInvitationCommand item)
    {
        return Task.FromResult(Validate(item));
    }
}