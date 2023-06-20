using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;

public class GetEmployerEnglishFractionHistoryQueryValidator : IValidator<GetEmployerEnglishFractionHistoryQuery>
{
    private readonly IMembershipRepository _membershipRepository;

    public GetEmployerEnglishFractionHistoryQueryValidator(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public ValidationResult Validate(GetEmployerEnglishFractionHistoryQuery item)
    {
        throw new NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(GetEmployerEnglishFractionHistoryQuery item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.EmpRef))
        {
            validationResult.AddError(nameof(item.EmpRef));
        }
        if (string.IsNullOrEmpty(item.UserId))
        {
            validationResult.AddError(nameof(item.UserId));
        }
        if (string.IsNullOrEmpty(item.HashedAccountId))
        {
            validationResult.AddError(nameof(item.HashedAccountId));
        }

        if (!validationResult.IsValid())
        {
            return validationResult;
        }

        var result = await _membershipRepository.GetCaller(item.HashedAccountId, item.UserId);

        validationResult.IsUnauthorized = result == null;

        return validationResult;
    }
}