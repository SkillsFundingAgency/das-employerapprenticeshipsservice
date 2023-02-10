namespace SFA.DAS.EmployerAccounts.Queries.GetVacancies;

public class GetVacanciesRequestValidator : IValidator<GetVacanciesRequest>
{
    public ValidationResult Validate(GetVacanciesRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.ExternalUserId))
        {
            validationResult.AddError(nameof(item.ExternalUserId), "ExternalUserId has not been supplied");
        }

        if (string.IsNullOrEmpty(item.HashedAccountId))
        {
            validationResult.ValidationDictionary.Add(nameof(item.HashedAccountId),
                "Account Id must be set.");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetVacanciesRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}