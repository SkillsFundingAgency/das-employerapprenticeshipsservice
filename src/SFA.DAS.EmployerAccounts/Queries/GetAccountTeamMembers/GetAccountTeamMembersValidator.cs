using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;

public class GetAccountTeamMembersValidator : IValidator<GetAccountTeamMembersQuery>
{
    private readonly IMembershipRepository _membershipRepository;

    public GetAccountTeamMembersValidator(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public ValidationResult Validate(GetAccountTeamMembersQuery item)
    {
        throw new NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(GetAccountTeamMembersQuery item)
    {
        var validationResult = new ValidationResult();
        if (string.IsNullOrEmpty(item.ExternalUserId))
        {
            validationResult.AddError(nameof(item.ExternalUserId), "UserId has not been supplied");
        }
        if (string.IsNullOrEmpty(item.HashedAccountId))
        {
            validationResult.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
        }

        if (validationResult.IsValid())
        {
            var member = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);
            if (member == null)
            {
                validationResult.AddError(nameof(member), "Unauthorised: User not connected to account");
                validationResult.IsUnauthorized = true;
            }
        }

        return validationResult;
    }
}