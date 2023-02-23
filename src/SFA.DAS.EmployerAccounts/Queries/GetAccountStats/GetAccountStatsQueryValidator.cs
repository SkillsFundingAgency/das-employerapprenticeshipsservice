using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountStats;

public class GetAccountStatsQueryValidator : IValidator<GetAccountStatsQuery>
{
    private readonly IMembershipRepository _repository;

    public GetAccountStatsQueryValidator(IMembershipRepository repository)
    {
        _repository = repository;
    }

    public ValidationResult Validate(GetAccountStatsQuery item)
    {
        throw new System.NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(GetAccountStatsQuery item)
    {
        var validationResult = new ValidationResult();
        if (string.IsNullOrEmpty(item.ExternalUserId))
        {
            validationResult.AddError(nameof(item.ExternalUserId), "UserId has not been supplied");
        }
        if (item.AccountId <= 0)
        {
            validationResult.AddError(nameof(item.AccountId), "AccountId has not been supplied");
        }

        if (validationResult.IsValid())
        {
            var member = await _repository.GetCaller(item.AccountId, item.ExternalUserId);
            if (member == null)
            {
                validationResult.AddError(nameof(member), "Unauthorised: User not connected to account");
                validationResult.IsUnauthorized = true;
            }
        }

        return validationResult;
    }
}