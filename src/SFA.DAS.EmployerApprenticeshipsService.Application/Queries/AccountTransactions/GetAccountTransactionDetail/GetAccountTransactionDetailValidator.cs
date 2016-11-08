using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail
{
    public class GetAccountTransactionDetailValidator : IValidator<GetAccountTransactionDetailQuery>
    {
        public ValidationResult Validate(GetAccountTransactionDetailQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.Id == 0)
            {
                validationResult.AddError(nameof(item.Id), "Id has not been supplied");
            }
            
            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetAccountTransactionDetailQuery item)
        {
            throw new NotImplementedException();
        }
    }
}