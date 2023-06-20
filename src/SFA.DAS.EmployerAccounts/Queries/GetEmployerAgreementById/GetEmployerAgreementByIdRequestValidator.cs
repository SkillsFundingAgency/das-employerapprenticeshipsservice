namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;

public class GetEmployerAgreementByIdRequestValidator : IValidator<GetEmployerAgreementByIdRequest>
{
    public ValidationResult Validate(GetEmployerAgreementByIdRequest item)
    {
        var validationResults = new Dictionary<string,string>();

        if (item.AgreementId <= 0)
        {
            validationResults.Add(nameof(item.AgreementId), "Hashed agreement ID must be populated");
        }

        return new ValidationResult
        {
            ValidationDictionary = validationResults
        };
    }

    public Task<ValidationResult> ValidateAsync(GetEmployerAgreementByIdRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}