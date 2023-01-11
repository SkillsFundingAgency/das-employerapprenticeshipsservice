using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;

public class GetUserAccountRoleValidator : IValidator<GetUserAccountRoleQuery>
{
    public ValidationResult Validate(GetUserAccountRoleQuery query)
    {
        var result = new ValidationResult();

        if (string.IsNullOrEmpty(query.ExternalUserId))
        {
            result.AddError(nameof(query.ExternalUserId), "ExternalUserId has not been supplied");
        }

        if (string.IsNullOrEmpty(query.HashedAccountId))
        {
            result.AddError(nameof(query.HashedAccountId), "HashedAccountId has not been supplied");
        }

        return result;
    }

    public Task<ValidationResult> ValidateAsync(GetUserAccountRoleQuery item)
    {
        throw new NotImplementedException();
    }
}