namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId;

public class GetEmployerAgreementsByAccountIdRequestValidator : IValidator<GetEmployerAgreementsByAccountIdRequest>
{
    public ValidationResult Validate(GetEmployerAgreementsByAccountIdRequest item)
    {
        var validationResults = new Dictionary<string,string>();

        if (item.AccountId <= 0)
        {
            validationResults.Add(nameof(item.AccountId), "Account Id must be populated");
        }

        return new ValidationResult
        {
            ValidationDictionary = validationResults
        };
    }

    public Task<ValidationResult> ValidateAsync(GetEmployerAgreementsByAccountIdRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}