namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;

public class GetApprenticeshipsValidator : IValidator<GetApprenticeshipsRequest>
{
    public ValidationResult Validate(GetApprenticeshipsRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.ExternalUserId))
        {
            validationResult.AddError(nameof(item.ExternalUserId), "ExternalUserId has not been supplied");
        }

        if (item.AccountId <= 0)
        {
            validationResult.ValidationDictionary.Add(nameof(item.AccountId),
                "Account Id must be populated.");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetApprenticeshipsRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}