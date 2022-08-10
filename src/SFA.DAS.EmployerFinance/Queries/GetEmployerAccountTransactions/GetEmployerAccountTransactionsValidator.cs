using SFA.DAS.Validation;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetEmployerAccountTransactions
{
    public class GetEmployerAccountTransactionsValidator : IValidator<GetEmployerAccountTransactionsQuery>
    {       
        public ValidationResult Validate(GetEmployerAccountTransactionsQuery item)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerAccountTransactionsQuery item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                result.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
            }
          
            return result;
        }
    }
}