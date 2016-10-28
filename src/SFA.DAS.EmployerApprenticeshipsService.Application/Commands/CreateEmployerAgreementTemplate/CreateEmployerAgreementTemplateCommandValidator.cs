using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.CreateEmployerAgreementTemplate
{
    public class CreateEmployerAgreementTemplateCommandValidator : IValidator<CreateEmployerAgreementTemplateCommand>
    {
        public ValidationResult Validate(CreateEmployerAgreementTemplateCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.TemplateRef))
                validationResult.AddError("Ref", "No Ref supplied");

            if (string.IsNullOrWhiteSpace(item.Text))
                validationResult.AddError("Text", "No Text supplied");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(CreateEmployerAgreementTemplateCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}