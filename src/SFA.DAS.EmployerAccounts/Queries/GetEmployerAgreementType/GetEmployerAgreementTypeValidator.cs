using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementType
{
    public class GetEmployerAgreementTypeValidator : IValidator<GetEmployerAgreementTypeRequest> 
    {
        public ValidationResult Validate(GetEmployerAgreementTypeRequest item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerAgreementTypeRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAgreementId))
            {
                validationResult.AddError(nameof(item.HashedAgreementId),"HashedAgreementId has not been supplied");
            }
                
            return validationResult;
        }
    }
}
