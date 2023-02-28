namespace SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember;

public class RemoveTeamMemberCommandValidator : IValidator<RemoveTeamMemberCommand>
{
    public ValidationResult Validate(RemoveTeamMemberCommand item)
    {
        var validationResult = new ValidationResult();

        if (item.UserId == 0)
            validationResult.AddError(nameof(item.UserId), "No UserId supplied");

        if (string.IsNullOrEmpty(item.HashedAccountId))
            validationResult.AddError(nameof(item.HashedAccountId), "No HashedAccountId supplied");

        if (string.IsNullOrWhiteSpace(item.ExternalUserId))
            validationResult.AddError(nameof(item.ExternalUserId), "No ExternalUserId supplied");

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(RemoveTeamMemberCommand item)
    {
        return Task.FromResult(Validate(item));
    }
}