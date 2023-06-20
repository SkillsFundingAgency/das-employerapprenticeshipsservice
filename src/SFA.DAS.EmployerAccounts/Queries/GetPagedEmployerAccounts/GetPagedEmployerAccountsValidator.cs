namespace SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;

public class GetPagedEmployerAccountsValidator : IValidator<GetPagedEmployerAccountsQuery>
{
    public ValidationResult Validate(GetPagedEmployerAccountsQuery item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.ToDate))
        {
            validationResult.AddError(nameof(item.ToDate), "ToDate has not been supplied");
        }

        if (item.PageSize == 0)
        {
            validationResult.AddError(nameof(item.PageSize), "PageSize has not been supplied");
        }

        if (item.PageNumber == 0)
        {
            validationResult.AddError(nameof(item.PageNumber), "PageNumber has not been supplied");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetPagedEmployerAccountsQuery item)
    {
        return Task.FromResult(Validate(item));
    }
}