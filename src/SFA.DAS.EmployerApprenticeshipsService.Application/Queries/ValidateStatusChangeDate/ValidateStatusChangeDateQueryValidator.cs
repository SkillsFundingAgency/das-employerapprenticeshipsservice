using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.ValidateStatusChangeDate
{
    public sealed class ValidateStatusChangeDateQueryValidator : IValidator<ValidateStatusChangeDateQuery>
    {
        public ValidationResult Validate(ValidateStatusChangeDateQuery item)
        {
            var result = new ValidationResult();

            if (item.AccountId < 1)
            {
                result.AddError(nameof(item.AccountId), "AccountId Id has not been supplied");
            }

            if (item.ApprenticeshipId < 1)
            {
                result.AddError(nameof(item.ApprenticeshipId), "Apprenticeship Id has not been supplied");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(ValidateStatusChangeDateQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
