namespace SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;

public class GetSingleCohortRequestValidator : IValidator<GetSingleCohortRequest>
{
    public ValidationResult Validate(GetSingleCohortRequest item)
    {
        var validationResult = new ValidationResult();

        if (item.AccountId <= 0)
        {
            validationResult.AddError(nameof(item.AccountId), "AccountId has not been supplied");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetSingleCohortRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}