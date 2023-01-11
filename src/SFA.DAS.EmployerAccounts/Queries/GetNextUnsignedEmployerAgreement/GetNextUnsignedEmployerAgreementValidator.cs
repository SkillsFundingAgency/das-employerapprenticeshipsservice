using SFA.DAS.Validation;

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
        var task = Task.Run(async () => await ValidateAsync(item));
        return task.Result;
    }

    public async Task<ValidationResult> ValidateAsync(GetNextUnsignedEmployerAgreementRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.ExternalUserId))
        {
            validationResult.AddError(nameof(item.ExternalUserId),"ExternalUserId has not been supplied");
        }
        if (string.IsNullOrEmpty(item.HashedAccountId))
        {
            validationResult.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
        }

        if (!validationResult.IsValid())
        {
            return validationResult;
        }

        var membership = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);

        if (membership == null)
        {
            validationResult.IsUnauthorized = true;
        }
                
        return validationResult;
    }
}