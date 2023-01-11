using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;

public class GetOrganisationAgreementsValidator : IValidator<GetOrganisationAgreementsRequest>
{
    public ValidationResult Validate(GetOrganisationAgreementsRequest item)
    {
        throw new NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(GetOrganisationAgreementsRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.AccountLegalEntityHashedId))
        {
            validationResult.AddError(nameof(item.AccountLegalEntityHashedId));
        }

        return validationResult;
    }
}