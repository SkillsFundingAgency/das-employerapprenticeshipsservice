namespace SFA.DAS.EmployerAccounts.Queries.GetUserByRef;

public class GetUserByRefQueryValidator : IValidator<GetUserByRefQuery>
{
    public ValidationResult Validate(GetUserByRefQuery item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.UserRef))
        {
            validationResult.AddError(nameof(item.UserRef), "User ref must not be empty or null");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetUserByRefQuery item)
    {
        return Task.FromResult(Validate(item));
    }
}