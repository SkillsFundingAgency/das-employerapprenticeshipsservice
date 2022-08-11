﻿using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetMinimumSignedAgreementVersion
{
    public class GetMinimumSignedAgreementVersionQueryValidator : IValidator<GetMinimumSignedAgreementVersionQuery>
    {
        public GetMinimumSignedAgreementVersionQueryValidator()
        {
        }

        public ValidationResult Validate(GetMinimumSignedAgreementVersionQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId <= 0)
            {
                validationResult.AddError(nameof(item.AccountId));
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetMinimumSignedAgreementVersionQuery item)
        {
            throw new System.NotImplementedException();
        }
    }
}