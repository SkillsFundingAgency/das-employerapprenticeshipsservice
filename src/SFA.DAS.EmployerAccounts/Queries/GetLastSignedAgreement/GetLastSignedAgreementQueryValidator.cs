using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;

public class GetLastSignedAgreementQueryValidator : IValidator<GetLastSignedAgreementRequest>
{
    public GetLastSignedAgreementQueryValidator()
    {
    }

    public ValidationResult Validate(GetLastSignedAgreementRequest item)
    {
        var task = Task.Run(async () => await ValidateAsync(item));
        return task.Result;
    }

    public async Task<ValidationResult> ValidateAsync(GetLastSignedAgreementRequest item)
    {
        var validationResult = new ValidationResult();

        if (item.AccountLegalEntityId <= 0)
        {
            validationResult.AddError(nameof(item.AccountLegalEntityId));
        }

        return validationResult;
    }
}