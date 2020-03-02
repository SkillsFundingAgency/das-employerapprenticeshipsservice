using SFA.DAS.Validation;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountCohort
{
    public class GetAccountCohortRequestValidator : IValidator<GetAccountCohortRequest>
    {
        public ValidationResult Validate(GetAccountCohortRequest item)
        {
            var validationResult = new ValidationResult();

            if (item.HashedAccountId == String.Empty)
            {
                validationResult.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetAccountCohortRequest item)
        {
            throw new NotImplementedException();
        }
    }
}
