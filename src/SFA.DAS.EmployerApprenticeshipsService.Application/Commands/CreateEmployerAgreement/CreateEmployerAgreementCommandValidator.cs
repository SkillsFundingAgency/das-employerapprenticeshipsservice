using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.CreateEmployerAgreement
{
    public class CreateEmployerAgreementTemplateCommandValidator : IValidator<CreateEmployerAgreementCommand>
    {
        public ValidationResult Validate(CreateEmployerAgreementCommand item)
        {
            var validationResult = new ValidationResult();

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(CreateEmployerAgreementCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}