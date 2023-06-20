using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;

public class GetNextUnsignedEmployerAgreementValidator : IValidator<GetNextUnsignedEmployerAgreementRequest> 
{
    private readonly IMembershipRepository _membershipRepository;

    public GetNextUnsignedEmployerAgreementValidator(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public ValidationResult Validate(GetNextUnsignedEmployerAgreementRequest item)
    {
        return ValidateAsync(item).Result;
    }

    public async Task<ValidationResult> ValidateAsync(GetNextUnsignedEmployerAgreementRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.ExternalUserId))
        {
            validationResult.AddError(nameof(item.ExternalUserId));
        }
        if (item.AccountId <= 0)
        {
            validationResult.AddError(nameof(item.AccountId));
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