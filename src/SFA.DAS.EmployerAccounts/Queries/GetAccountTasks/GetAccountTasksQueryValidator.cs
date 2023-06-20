namespace SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;

public class GetAccountTasksQueryValidator : IValidator<GetAccountTasksQuery>
{
    public ValidationResult Validate(GetAccountTasksQuery item)
    {
        var validationResult = new ValidationResult();
        if (item == null)
        {
            validationResult.AddError(nameof(GetAccountTasksQuery), "Message must be supplied");
            return validationResult;
        }
        if (item.AccountId == default(int))
        {
            validationResult.AddError(nameof(item.AccountId), "Account id must be supplied");
        }
        if (string.IsNullOrWhiteSpace(item.ExternalUserId))
        {
            validationResult.AddError(nameof(item.ExternalUserId), "External user id must be supplied");
        }
        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetAccountTasksQuery item)
    {
        return Task.FromResult(Validate(item));
    }
}