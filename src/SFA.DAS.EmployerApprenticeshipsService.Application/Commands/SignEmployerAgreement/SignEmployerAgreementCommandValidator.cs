using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SignEmployerAgreement
{
    public class SignEmployerAgreementCommandValidator : IValidator<SignEmployerAgreementCommand>
    {
        public ValidationResult Validate(SignEmployerAgreementCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAgreementId))
                validationResult.AddError("AgreementId", "No AgreementId supplied");

            if (string.IsNullOrEmpty(item.HashedId))
                validationResult.AddError("HashedId", "No HashedId supplied");

            if (string.IsNullOrWhiteSpace(item.ExternalUserId))
                validationResult.AddError("ExternalUserId", "No ExternalUserId supplied");

            if (item.SignedDate == default(DateTime))
                validationResult.AddError("SignedDate", "No SignedDate supplied");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(SignEmployerAgreementCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}