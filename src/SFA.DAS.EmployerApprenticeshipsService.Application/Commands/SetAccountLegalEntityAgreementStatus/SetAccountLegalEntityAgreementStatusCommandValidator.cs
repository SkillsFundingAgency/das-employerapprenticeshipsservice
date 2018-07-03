using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.SetAccountLegalEntityAgreementStatus
{
    public class SetAccountLegalEntityAgreementStatusCommandValidator : IValidator<SetAccountLegalEntityAgreementStatusCommand>
    {
        public ValidationResult Validate(SetAccountLegalEntityAgreementStatusCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId < 1)
            {
                validationResult.AddError(nameof(item.AccountId),"A value must be supplied");
            }

            if (item.LegalEntityId < 1)
            {
                validationResult.AddError(nameof(item.LegalEntityId), "A value must be supplied");
            }
            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(SetAccountLegalEntityAgreementStatusCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}