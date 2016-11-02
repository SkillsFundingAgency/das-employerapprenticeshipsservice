using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances
{
    public class GetAccountBalancesValidator : IValidator<GetAccountBalancesRequest>
    {
        public ValidationResult Validate(GetAccountBalancesRequest item)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValidationResult> ValidateAsync(GetAccountBalancesRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}