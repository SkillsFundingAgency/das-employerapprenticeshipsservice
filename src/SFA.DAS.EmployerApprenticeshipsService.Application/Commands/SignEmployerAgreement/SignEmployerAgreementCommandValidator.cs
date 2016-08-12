using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SignEmployerAgreement
{
    public class SignEmployerAgreementCommandValidator : IValidator<SignEmployerAgreementCommand>
    {
        public ValidationResult Validate(SignEmployerAgreementCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.AgreementId == 0)
                validationResult.AddError("AgreementId", "No AgreementId supplied");

            if (item.AccountId == 0)
                validationResult.AddError("AccountId", "No AccountId supplied");

            if (string.IsNullOrWhiteSpace(item.ExternalUserId))
                validationResult.AddError("ExternalUserId", "No ExternalUserId supplied");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(SignEmployerAgreementCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}