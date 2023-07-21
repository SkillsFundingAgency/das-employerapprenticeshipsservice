namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;

public class GetAccountPayeSchemesQueryValidator : IValidator<GetAccountPayeSchemesQuery>
{
    public ValidationResult Validate(GetAccountPayeSchemesQuery item)
    {
        throw new System.NotImplementedException();
    }

    public Task<ValidationResult> ValidateAsync(GetAccountPayeSchemesQuery query)
    {
        var validationResult = new ValidationResult();

        if (query.AccountId <= 0)
        {
            validationResult.ValidationDictionary.Add(nameof(query.AccountId), "Account ID has not been supplied");
        }

        return Task.FromResult(validationResult);
    }
}