using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.ChangeTeamMemberRole;

public class ChangeTeamMemberRoleCommandValidator : IValidator<ChangeTeamMemberRoleCommand>
{
    public ValidationResult Validate(ChangeTeamMemberRoleCommand item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.HashedAccountId))
            validationResult.AddError("HashedId", "No HashedId supplied");

        if (string.IsNullOrWhiteSpace(item.Email))
            validationResult.AddError("Email", "No Email supplied");

        if (item.Role == Role.None)
            validationResult.AddError("Role", "No Role supplied");

        if (string.IsNullOrWhiteSpace(item.ExternalUserId))
            validationResult.AddError("ExternalUserId", "No ExternalUserId supplied");

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(ChangeTeamMemberRoleCommand item)
    {
        return Task.FromResult(Validate(item));
    }
}