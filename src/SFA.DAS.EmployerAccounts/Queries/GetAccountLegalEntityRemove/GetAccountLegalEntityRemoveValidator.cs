using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove;

public class GetAccountLegalEntityRemoveValidator : IValidator<GetAccountLegalEntityRemoveRequest>
{
    private readonly IMembershipRepository _membershipRepository;

    public GetAccountLegalEntityRemoveValidator(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public ValidationResult Validate(GetAccountLegalEntityRemoveRequest item)
    {
        throw new NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(GetAccountLegalEntityRemoveRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.HashedAccountId))
        {
            validationResult.AddError(nameof(item.HashedAccountId));
        }
        if (string.IsNullOrEmpty(item.UserId))
        {
            validationResult.AddError(nameof(item.UserId));
        }
        if (string.IsNullOrEmpty(item.HashedAccountLegalEntityId))
        {
            validationResult.AddError(nameof(item.HashedAccountLegalEntityId));
        }

        if (!validationResult.IsValid())
        {
            return validationResult;
        }

        var user = await _membershipRepository.GetCaller(item.HashedAccountId, item.UserId);

        if (user == null || user.Role != Role.Owner)
        {
            validationResult.IsUnauthorized = true;
        }

        return validationResult;
    }
}