using SFA.DAS.EAS.Application.Validation;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers
{
    public class RefreshAccountTransfersCommandValidator : IValidator<RefreshAccountTransfersCommand>
    {
        public ValidationResult Validate(RefreshAccountTransfersCommand item)
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

        public Task<ValidationResult> ValidateAsync(RefreshAccountTransfersCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}