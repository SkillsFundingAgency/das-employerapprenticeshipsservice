namespace SFA.DAS.EmployerAccounts.Queries.GetHmrcEmployerInformation;

public class GetHmrcEmployerInformationValidator : IValidator<GetHmrcEmployerInformationQuery>
{
    public ValidationResult Validate(GetHmrcEmployerInformationQuery item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrWhiteSpace(item.AuthToken))
        {
            validationResult.AddError(nameof(item.AuthToken),"AuthToken has not been supplied");
        }
            
        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetHmrcEmployerInformationQuery item)
    {
        return Task.FromResult(Validate(item));
    }
}