using SFA.DAS.Validation;

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

        if (string.IsNullOrEmpty(item.HashedAccountId))
        {
            validationResult.ValidationDictionary.Add(nameof(item.HashedAccountId),
                "Hashed Account Id cannot be null or empty.");
        }

        return validationResult;
    }

    public Task<ValidationResult> ValidateAsync(GetReservationsRequest item)
    {
        return Task.FromResult(Validate(item));
    }
}