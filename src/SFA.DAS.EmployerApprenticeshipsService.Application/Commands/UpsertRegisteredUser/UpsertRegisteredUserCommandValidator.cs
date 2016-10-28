using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommandValidator : IValidator<UpsertRegisteredUserCommand>
    {
        public ValidationResult Validate(UpsertRegisteredUserCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.EmailAddress))
                validationResult.AddError("Email", "No Email supplied");

            if (string.IsNullOrWhiteSpace(item.FirstName))
                validationResult.AddError("FirstName", "No FirstName supplied");

            if (string.IsNullOrWhiteSpace(item.LastName))
                validationResult.AddError("LastName", "No LastName supplied");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(UpsertRegisteredUserCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}
