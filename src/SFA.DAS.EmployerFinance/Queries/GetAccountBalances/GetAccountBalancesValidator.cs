using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetAccountBalances
{
    public class GetAccountBalancesValidator : IValidator<GetAccountBalancesRequest>
    {
        public ValidationResult Validate(GetAccountBalancesRequest item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountIds == null || !item.AccountIds.Any())
            {
                validationResult.AddError(nameof(item.AccountIds), "AccountIds has not been supplied");
            }

            return validationResult;

        }

        public Task<ValidationResult> ValidateAsync(GetAccountBalancesRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}
