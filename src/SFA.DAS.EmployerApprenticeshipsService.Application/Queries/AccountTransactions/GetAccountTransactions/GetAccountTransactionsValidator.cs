using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions
{
    public class GetAccountTransactionsValidator : IValidator<GetAccountTransactionsRequest>
    {
        public ValidationResult Validate(GetAccountTransactionsRequest item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId == 0)
            {
                validationResult.AddError(nameof(item.AccountId),"AccountId has not been supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetAccountTransactionsRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}