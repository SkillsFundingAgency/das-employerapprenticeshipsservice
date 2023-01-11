using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisations;

public class GetOrganisationsValidator : IValidator<GetOrganisationsRequest>
{
    public ValidationResult Validate(GetOrganisationsRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.SearchTerm))
        {
            validationResult.AddError(nameof(item.SearchTerm));
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetOrganisationsRequest item)
    {
        throw new System.NotImplementedException();
    }
}