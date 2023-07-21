namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationById;

public class GetOrganisationByIdValidator : IValidator<GetOrganisationByIdRequest>
{
    public ValidationResult Validate(GetOrganisationByIdRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.Identifier))
        {
            validationResult.AddError(nameof(item.Identifier));
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetOrganisationByIdRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}