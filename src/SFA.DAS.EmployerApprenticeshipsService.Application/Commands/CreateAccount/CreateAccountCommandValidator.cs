using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateAccount
{
    public class CreateAccountCommandValidator : IValidator<CreateAccountCommand>
    {
        public ValidationResult Validate(CreateAccountCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.ExternalUserId))
                validationResult.AddError("UserId", "No UserId supplied");

            if (string.IsNullOrWhiteSpace(item.CompanyNumber))
                validationResult.AddError("CompanyNumber", "No CompanyNumber supplied");

            if (string.IsNullOrWhiteSpace(item.CompanyName))
                validationResult.AddError("CompanyName", "No CompanyName supplied");

            if (string.IsNullOrWhiteSpace(item.EmployerRef))
                validationResult.AddError("EmployerRef", "No EmployerRef supplied");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(CreateAccountCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}