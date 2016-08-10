using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeWithExistingLegalEntity
{
    public class AddPayeToAccountForExistingLegalEntityCommandValidator : IValidator<AddPayeToAccountForExistingLegalEntityCommand>
    {
        public ValidationResult Validate(AddPayeToAccountForExistingLegalEntityCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId == 0)
                validationResult.AddError("AccountId", "No AccountId supplied");

            if (item.LegalEntityId == 0)
                validationResult.AddError("LegalEntityId", "No LegalEntityId supplied");

            if (string.IsNullOrWhiteSpace(item.EmpRef))
                validationResult.AddError("EmpRef", "No EmpRef supplied");

            if (string.IsNullOrWhiteSpace(item.ExternalUserId))
                validationResult.AddError("ExternalUserId", "No ExternalUserId supplied");

            return validationResult;
        }
    }
}