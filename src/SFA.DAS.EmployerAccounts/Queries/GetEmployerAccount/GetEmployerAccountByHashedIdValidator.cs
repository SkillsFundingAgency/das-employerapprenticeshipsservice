using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;

public class GetEmployerAccountByHashedIdValidator : IValidator<GetEmployerAccountByHashedIdQuery>
{
    private readonly IMembershipRepository _membershipRepository;

    public GetEmployerAccountByHashedIdValidator(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public ValidationResult Validate(GetEmployerAccountByHashedIdQuery item)
    {
        throw new NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(GetEmployerAccountByHashedIdQuery item)
    {
        var result = new ValidationResult();

        if (string.IsNullOrEmpty(item.UserId))
        {
            result.AddError(nameof(item.UserId), "UserId has not been supplied");
        }

        if (string.IsNullOrEmpty(item.AccountId))
        {
            result.AddError(nameof(item.AccountId), "HashedAccountId has not been supplied");
        }

        if (result.IsValid())
        {
            var membership = await _membershipRepository.GetCaller(item.AccountId, item.UserId);

            if (membership == null)
                result.IsUnauthorized = true;
        }

        return result;
    }
}