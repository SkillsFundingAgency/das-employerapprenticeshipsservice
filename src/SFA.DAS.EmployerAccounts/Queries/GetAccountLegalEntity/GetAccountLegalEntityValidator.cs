namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntity;

public class GetAccountLegalEntityValidator : IValidator<GetAccountLegalEntityRequest>
{
    public ValidationResult Validate(GetAccountLegalEntityRequest item)
    {
        var validationResult = new ValidationResult();

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetAccountLegalEntityRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}