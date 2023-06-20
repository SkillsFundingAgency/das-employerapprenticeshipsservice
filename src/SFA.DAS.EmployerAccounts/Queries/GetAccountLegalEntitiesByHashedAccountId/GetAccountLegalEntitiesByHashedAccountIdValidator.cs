namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;

public class GetAccountLegalEntitiesByHashedAccountIdValidator : IValidator<GetAccountLegalEntitiesByHashedAccountIdRequest>
{
    public ValidationResult Validate(GetAccountLegalEntitiesByHashedAccountIdRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.HashedAccountId))
        {
            validationResult.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetAccountLegalEntitiesByHashedAccountIdRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}