using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;

public class CreateUserAccountCommandValidator : IValidator<CreateUserAccountCommand>
{
    public Task<ValidationResult> ValidateAsync(CreateUserAccountCommand item)
    {
        throw new System.NotImplementedException();
    }

    public ValidationResult Validate(CreateUserAccountCommand item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrWhiteSpace(item.ExternalUserId))
            validationResult.AddError("UserId", "No UserId supplied");
      
        if (string.IsNullOrWhiteSpace(item.OrganisationName))
            validationResult.AddError(nameof(item.OrganisationName), "No organisation name supplied");
  
        return validationResult;
    }
}