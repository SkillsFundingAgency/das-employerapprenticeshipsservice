using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransferAllowance
{
    public class GetTransferAllowanceValidator : IValidator<GetAccountTransferAllowanceRequest>
    {
        public ValidationResult Validate(GetAccountTransferAllowanceRequest item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId == 0)
            {
                validationResult.AddError(nameof(item.AccountId),"AccountId has not been supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(GetAccountTransferAllowanceRequest item)
        {
            throw new System.NotImplementedException();
        }
    }
}