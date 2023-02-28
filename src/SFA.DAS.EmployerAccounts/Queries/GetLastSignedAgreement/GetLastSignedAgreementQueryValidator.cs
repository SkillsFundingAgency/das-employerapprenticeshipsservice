namespace SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;

public class GetLastSignedAgreementQueryValidator : IValidator<GetLastSignedAgreementRequest>
{
    public ValidationResult Validate(GetLastSignedAgreementRequest item)
    {
        var validationResult = new ValidationResult();

        if (item.AccountLegalEntityId <= 0)
        {
            validationResult.AddError(nameof(item.AccountLegalEntityId));
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetLastSignedAgreementRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}