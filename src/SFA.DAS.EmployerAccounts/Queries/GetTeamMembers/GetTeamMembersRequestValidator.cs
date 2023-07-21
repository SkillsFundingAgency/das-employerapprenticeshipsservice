namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;

public class GetTeamMembersRequestValidator : IValidator<GetTeamMembersRequest>
{
    public ValidationResult Validate(GetTeamMembersRequest item)
    {
        var result = new ValidationResult();

        if (item.AccountId <= 0)
        {
            result.ValidationDictionary.Add(nameof(item.AccountId),
                "Account Id must have a value.");
        }

        return result;
    }

    public Task<ValidationResult> ValidateAsync(GetTeamMembersRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}