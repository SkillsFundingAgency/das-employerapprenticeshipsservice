using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;

public class GetAccountEmployerAgreementsValidator : IValidator<GetAccountEmployerAgreementsRequest>
{
    private readonly IMembershipRepository _membershipRepository;

    public GetAccountEmployerAgreementsValidator(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public ValidationResult Validate(GetAccountEmployerAgreementsRequest item)
    {
        throw new NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(GetAccountEmployerAgreementsRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.ExternalUserId))
        {
            validationResult.AddError(nameof(item.ExternalUserId), "ExternalUserId has not been supplied");
        }
        if (item.AccountId <= 0)
        {
            validationResult.AddError(nameof(item.AccountId), "AccountId has not been supplied");
        }

        if (!validationResult.IsValid())
        {
            return validationResult;
        }

        var membership = await _membershipRepository.GetCaller(item.AccountId, item.ExternalUserId);

        if (membership == null)
        {
            validationResult.IsUnauthorized = true;
        }


        return validationResult;
    }
}