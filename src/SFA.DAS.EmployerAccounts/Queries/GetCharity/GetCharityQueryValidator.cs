namespace SFA.DAS.EmployerAccounts.Queries.GetCharity;

public class GetCharityQueryValidator : IValidator<GetCharityQueryRequest>
{
    public ValidationResult Validate(GetCharityQueryRequest item)
    {
        var validationResult = new ValidationResult();

        if (item.RegistrationNumber == 0)
        {
            validationResult.AddError(nameof(item.RegistrationNumber));
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetCharityQueryRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}