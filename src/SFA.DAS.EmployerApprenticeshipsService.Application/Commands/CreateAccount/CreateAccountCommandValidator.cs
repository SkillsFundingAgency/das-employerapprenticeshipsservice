using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateAccount
{
    public class CreateAccountCommandValidator : IValidator<CreateAccountCommand>
    {
        private readonly IEmployerSchemesRepository _employerSchemesRepository;

        public CreateAccountCommandValidator(IEmployerSchemesRepository employerSchemesRepository)
        {
            _employerSchemesRepository = employerSchemesRepository;
        }

        public ValidationResult Validate(CreateAccountCommand item)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(CreateAccountCommand item)
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

            if (validationResult.IsValid())
            {
                var result = await _employerSchemesRepository.GetSchemeByRef(item.EmployerRef);
                if (result != null)
                {
                    validationResult.AddError(nameof(item.EmployerRef),"Scheme already in use");
                }
            }

            return validationResult;
        }
    }
}