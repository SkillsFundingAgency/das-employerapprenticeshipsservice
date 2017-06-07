using System.Linq;
using System.Threading.Tasks;

using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Extensions;

namespace SFA.DAS.EAS.Application.Commands.UpdateProviderPaymentPriority
{
    public class UpdateProviderPaymentPriorityValidator : IValidator<UpdateProviderPaymentPriorityCommand>
    {
        public ValidationResult Validate(UpdateProviderPaymentPriorityCommand item)
        {
            var result = new ValidationResult();

            if (item.ProviderPriorityOrder == null || item.ProviderPriorityOrder.Count() < 2)
            {
                result.AddError("ProviderPriorityOrder", "Need more than 1 item to set provider payment priority");
                return result;
            }

            if (item.ProviderPriorityOrder.DistinctBy(m => m).Count() != item.ProviderPriorityOrder.Count())
            {
                result.AddError("ProviderPriorityOrder", $"Provider payment priority cannot contains duplication of provider id");
            }

            if (string.IsNullOrEmpty(item.UserId))
            {
                result.AddError($"{nameof(item.UserId)}", $"{nameof(item.UserId)} cannot be null or empty");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(UpdateProviderPaymentPriorityCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}