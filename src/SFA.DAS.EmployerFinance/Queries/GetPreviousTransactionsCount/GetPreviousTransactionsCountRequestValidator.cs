using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetPreviousTransactionsCount
{
    public class GetPreviousTransactionsCountRequestValidator : IValidator<GetPreviousTransactionsCountRequest>
    {
        public ValidationResult Validate(GetPreviousTransactionsCountRequest request)
        {
           throw new NotImplementedException();
        }

        public Task<ValidationResult> ValidateAsync(GetPreviousTransactionsCountRequest request)
        {
            var validationResult = new ValidationResult();

            if (request.AccountId <= 0)
            {
                validationResult.AddError(nameof(request.AccountId));
            }

            if (request.FromDate.Equals(default(DateTime)))
            {
                validationResult.AddError(nameof(request.FromDate));
            }

            return Task.FromResult(validationResult);
        }
    }
}
