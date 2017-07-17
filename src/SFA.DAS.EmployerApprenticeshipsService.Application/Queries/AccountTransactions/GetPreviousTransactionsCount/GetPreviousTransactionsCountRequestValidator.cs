using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetPreviousTransactionsCount
{
    public class GetPreviousTransactionsCountRequestValidator : IValidator<GetPreviousTransactionsCountRequest>
    {
        public ValidationResult Validate(GetPreviousTransactionsCountRequest request)
        {
           throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetPreviousTransactionsCountRequest request)
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

            return validationResult;
        }
    }
}
