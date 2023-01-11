using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;

public class GetTeamMembersRequestValidator : IValidator<GetTeamMembersRequest>
{
    public ValidationResult Validate(GetTeamMembersRequest item)
    {
        var result = new ValidationResult();

        if (string.IsNullOrEmpty(item.HashedAccountId))
        {
            result.ValidationDictionary.Add(nameof(item.HashedAccountId),
                "Hashed Account Id cannot be null or empty.");
        }

        return result;
    }

    public Task<ValidationResult> ValidateAsync(GetTeamMembersRequest item)
    {
        throw new System.NotImplementedException();
    }
}