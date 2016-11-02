using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactions
{
    public class GetAccountTransactionsValidator : IValidator<GetAccountTransactionsRequest>
    {
        public ValidationResult Validate(GetAccountTransactionsRequest item)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValidationResult> ValidateAsync(GetAccountTransactionsRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}