using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;

public class GetAccountPayeSchemesQueryValidator : IValidator<GetAccountPayeSchemesQuery>
{
    public ValidationResult Validate(GetAccountPayeSchemesQuery item)
    {
        throw new System.NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(GetAccountPayeSchemesQuery query)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(query.HashedAccountId))
        {
            validationResult.ValidationDictionary.Add(nameof(query.HashedAccountId), "Hashed account ID has not been supplied");
        }

        return validationResult;
    }
}