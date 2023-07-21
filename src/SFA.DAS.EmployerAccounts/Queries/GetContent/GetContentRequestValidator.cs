namespace SFA.DAS.EmployerAccounts.Queries.GetContent;

public class GetContentRequestValidator : IValidator<GetContentRequest>
{
    public ValidationResult Validate(GetContentRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.ContentType))
        {
            validationResult.AddError(nameof(item.ContentType), "Type has not been supplied");
        }
        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetContentRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}