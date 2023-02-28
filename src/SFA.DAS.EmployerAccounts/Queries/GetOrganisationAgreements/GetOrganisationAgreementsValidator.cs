namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;

public class GetOrganisationAgreementsValidator : IValidator<GetOrganisationAgreementsRequest>
{
    public ValidationResult Validate(GetOrganisationAgreementsRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.AccountLegalEntityHashedId))
        {
            validationResult.AddError(nameof(item.AccountLegalEntityHashedId));
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetOrganisationAgreementsRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}