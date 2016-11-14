using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData
{
    public class RefreshPaymentDataCommandValidator  : IValidator<RefreshPaymentDataCommand>
    {
        public ValidationResult Validate(RefreshPaymentDataCommand item)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValidationResult> ValidateAsync(RefreshPaymentDataCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}