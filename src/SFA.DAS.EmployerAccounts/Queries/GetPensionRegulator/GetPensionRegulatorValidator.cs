namespace SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;

public class GetPensionRegulatorValidator : IValidator<GetPensionRegulatorRequest>
{
    public ValidationResult Validate(GetPensionRegulatorRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.PayeRef))
        {
            validationResult.AddError(nameof(item.PayeRef));
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetPensionRegulatorRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}