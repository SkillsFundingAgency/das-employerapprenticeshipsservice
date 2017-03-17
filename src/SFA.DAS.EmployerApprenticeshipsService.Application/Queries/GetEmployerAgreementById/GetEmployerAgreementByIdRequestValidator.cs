using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreementById
{
    public class GetEmployerAgreementByIdRequestValidator : IValidator<GetEmployerAgreementByIdRequest>
    {
        public ValidationResult Validate(GetEmployerAgreementByIdRequest item)
        {
            var validationResults = new Dictionary<string,string>();

            if (string.IsNullOrEmpty(item.HashedAgreementId))
            {
                validationResults.Add(nameof(item.HashedAgreementId), "Hashed agreement ID must be populated");
            }

            return new ValidationResult
            {
                ValidationDictionary = validationResults
            };
        }

        public Task<ValidationResult> ValidateAsync(GetEmployerAgreementByIdRequest item)
        {
            throw new NotImplementedException();
        }
    }
}
