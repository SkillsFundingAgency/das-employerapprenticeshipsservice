namespace SFA.DAS.EmployerAccounts.Queries.GetUser;

public class GetUserQueryValidator : IValidator<GetUserQuery>
{
    public ValidationResult Validate(GetUserQuery item)
    {
        var result = new ValidationResult();

        if (item.UserId < 1)
        {
            result.AddError(nameof(item.UserId), "UserId has not been supplied");
        }

        return result;
    }

    public Task<ValidationResult> ValidateAsync(GetUserQuery item)
    {
        return Task.FromResult(Validate(item));
    }
}