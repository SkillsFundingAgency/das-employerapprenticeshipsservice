using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserByRef;

public class GetUserByRefQueryValidator : IValidator<GetUserByRefQuery>
{
    public ValidationResult Validate(GetUserByRefQuery query)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(query.UserRef))
        {
            validationResult.AddError(nameof(query.UserRef), "User ref must not be empty or null");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetUserByRefQuery query)
    {
        throw new NotImplementedException();
    }
}