using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;

public class GetSingleCohortRequestValidator : IValidator<GetSingleCohortRequest>
{
    public ValidationResult Validate(GetSingleCohortRequest item)
    {
        var validationResult = new ValidationResult();

        if (item.AccountId == String.Empty)
        {
            validationResult.AddError(nameof(item.AccountId), "HashedAccountId has not been supplied");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetSingleCohortRequest item)
    {
        throw new NotImplementedException();
    }
}