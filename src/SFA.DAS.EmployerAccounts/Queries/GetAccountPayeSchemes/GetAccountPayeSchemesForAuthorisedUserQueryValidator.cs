using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;

public class GetAccountPayeSchemesForAuthorisedUserQueryValidator : IValidator<GetAccountPayeSchemesForAuthorisedUserQuery>
{
    private readonly IMembershipRepository _membershipRepository;

    public GetAccountPayeSchemesForAuthorisedUserQueryValidator(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public ValidationResult Validate(GetAccountPayeSchemesForAuthorisedUserQuery query)
    {
        return
            ValidateAsync(query).Result;
    }

    public async Task<ValidationResult> ValidateAsync(GetAccountPayeSchemesForAuthorisedUserQuery query)
    {
        var validationResult = new ValidationResult();

        if (query.AccountId <= 0)
        {
            validationResult.ValidationDictionary.Add(nameof(query.AccountId), "Account ID has not been supplied");
        }

        if (string.IsNullOrEmpty(query.ExternalUserId))
        {
            validationResult.ValidationDictionary.Add(nameof(query.ExternalUserId), "User ID has not been supplied");
        }

        if (validationResult.IsValid())
        {
            var member = await _membershipRepository.GetCaller(query.AccountId, query.ExternalUserId);
            if (member == null)
            {
                validationResult.AddError(nameof(member), "Unauthorised: User not connected to account");
                validationResult.IsUnauthorized = true;
            }
        }

        return validationResult;
    }
}