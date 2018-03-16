using SFA.DAS.EAS.Application.Validation;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Commands.CreateTransferTransactions
{
    public class CreateTransferTransactionsCommandValidator : IValidator<CreateTransferTransactionsCommand>
    {
        public ValidationResult Validate(CreateTransferTransactionsCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId < 0)
            {
                validationResult.AddError(nameof(item.AccountId), "AccountId cannot be negative");
            }
            else if (item.AccountId == default(long))
            {
                validationResult.AddError(nameof(item.AccountId), "AccountId has not been supplied");
            }

            if (string.IsNullOrEmpty(item.PeriodEnd))
            {
                validationResult.AddError(nameof(item.PeriodEnd), "PeriodEnd has not been supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(CreateTransferTransactionsCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}