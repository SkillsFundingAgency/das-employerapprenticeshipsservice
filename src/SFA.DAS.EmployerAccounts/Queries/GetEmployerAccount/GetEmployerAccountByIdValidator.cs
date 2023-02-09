using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;

public class GetEmployerAccountByIdValidator : IValidator<GetEmployerAccountByIdQuery>
{
    private readonly IMembershipRepository _membershipRepository;

    public GetEmployerAccountByIdValidator(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public ValidationResult Validate(GetEmployerAccountByIdQuery item)
    {
        throw new NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(GetEmployerAccountByIdQuery item)
    {
        var result = new ValidationResult();

        if (string.IsNullOrEmpty(item.UserId))
        {
            result.AddError(nameof(item.UserId), "UserId has not been supplied");
        }

        if (item.AccountId <= 0)
        {
            result.AddError(nameof(item.AccountId), "AccountId has not been supplied");
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