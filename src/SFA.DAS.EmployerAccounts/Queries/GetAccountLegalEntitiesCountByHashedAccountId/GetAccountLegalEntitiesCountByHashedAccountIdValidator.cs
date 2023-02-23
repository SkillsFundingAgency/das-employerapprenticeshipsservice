namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesCountByHashedAccountId;

public class GetAccountLegalEntitiesCountByHashedAccountIdValidator : IValidator<GetAccountLegalEntitiesCountByHashedAccountIdRequest>
{
    public ValidationResult Validate(GetAccountLegalEntitiesCountByHashedAccountIdRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.HashedAccountId))
        {
            validationResult.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetAccountLegalEntitiesCountByHashedAccountIdRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}