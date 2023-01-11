using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;

public class GetEmployerAccountDetailByHashedIdValidator : IValidator<GetEmployerAccountDetailByHashedIdQuery>
{
    public ValidationResult Validate(GetEmployerAccountDetailByHashedIdQuery item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.HashedAccountId))
        {
            validationResult.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetEmployerAccountDetailByHashedIdQuery item)
    {
        throw new NotImplementedException();
    }
}