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

        if (query.AccountId <= 0)
        {
            result.AddError(nameof(query.AccountId), "AccountId has not been supplied");
        }

        return result;
    }

    public Task<ValidationResult> ValidateAsync(GetUserAccountRoleQuery item)
    {
        return Task.FromResult(Validate(item));
    }
}