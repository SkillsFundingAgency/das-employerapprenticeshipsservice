namespace SFA.DAS.EmployerAccounts.Queries.GetMinimumSignedAgreementVersion;

public class GetMinimumSignedAgreementVersionQueryValidator : IValidator<GetMinimumSignedAgreementVersionQuery>
{
    public ValidationResult Validate(GetMinimumSignedAgreementVersionQuery item)
    {
        var validationResult = new ValidationResult();

        if (item.AccountId <= 0)
        {
            validationResult.AddError(nameof(item.AccountId));
        }

        return validationResult;
    }

    public async Task<ValidationResult> ValidateAsync(GetMinimumSignedAgreementVersionQuery item)
    {
        return await Task.FromResult(Validate(item));
    }
}