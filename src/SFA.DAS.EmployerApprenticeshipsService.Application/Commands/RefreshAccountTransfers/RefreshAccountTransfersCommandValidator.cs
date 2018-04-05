using SFA.DAS.EAS.Application.Validation;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers
{
    public class RefreshAccountTransfersCommandValidator : IValidator<RefreshAccountTransfersCommand>
    {
        public ValidationResult Validate(RefreshAccountTransfersCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.ReceiverAccountId < 0)
            {
                validationResult.AddError(nameof(item.ReceiverAccountId), "ReceiverAccountId cannot be negative");
            }
            else if (item.ReceiverAccountId == default(long))
            {
                validationResult.AddError(nameof(item.ReceiverAccountId), "ReceiverAccountId has not been supplied");
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