using System.Threading.Tasks;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount
{
    public class GetEmployerAccountByHashedIdForAuthorisedUserValidator : IValidator<GetEmployerAccountByHashedIdQuery>
    {
        public ValidationResult Validate(GetEmployerAccountByHashedIdQuery item)
        {
            return ValidateAsync(item).Result;
        }

        public async Task<ValidationResult> ValidateAsync(GetEmployerAccountByHashedIdQuery item)
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