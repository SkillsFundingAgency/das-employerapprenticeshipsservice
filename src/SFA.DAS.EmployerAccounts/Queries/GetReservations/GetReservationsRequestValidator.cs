namespace SFA.DAS.EmployerAccounts.Queries.GetReservations;

public class GetReservationsRequestValidator : IValidator<GetReservationsRequest>
{
    public ValidationResult Validate(GetReservationsRequest item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrEmpty(item.ExternalUserId))
        {
            validationResult.AddError(nameof(item.ExternalUserId), "ExternalUserId has not been supplied");
        }

        if (item.AccountId <= 0)
        {
            validationResult.ValidationDictionary.Add(nameof(item.AccountId),
                "Account Id must be set.");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetReservationsRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}