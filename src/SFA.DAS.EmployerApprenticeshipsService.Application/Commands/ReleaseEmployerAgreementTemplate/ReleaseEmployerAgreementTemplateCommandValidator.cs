using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.ReleaseEmployerAgreementTemplate
{
    public class ReleaseEmployerAgreementTemplateCommandValidator : IValidator<ReleaseEmployerAgreementTemplateCommand>
    {
        public ValidationResult Validate(ReleaseEmployerAgreementTemplateCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.TemplateId == 0)
                validationResult.AddError("TemplateId", "No TemplateId supplied");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(ReleaseEmployerAgreementTemplateCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}