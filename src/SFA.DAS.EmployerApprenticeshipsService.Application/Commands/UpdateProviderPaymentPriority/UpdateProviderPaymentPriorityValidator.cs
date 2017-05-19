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

            if (item.Data == null || item.Data.Count() < 2)
            {
                result.AddError("PaymentPriority.NeedMoreThanOneItemsInList", "Need more than 1 item to set provider payment provider");
                return result;
            }

            if (item.Data.Any(m => m.PaymentPriority < 1))
            {
                result.AddError("PaymentPriority.LessThan1", "Provider payment priority cannot be less than 1");
            }

            if (item.Data.Any(m => m.PaymentPriority > item.Data.Count()))
            {
                result.AddError("PaymentPriority.GreaterThan", "Provider payment priority cannot have a priority order hight than the number of providers");
            }

            if (item.Data.DistinctBy(m => m.PaymentPriority).Count() != item.Data.Count())
            {
                var priorities  = string.Join(",", item.Data.Select(m => m.PaymentPriority));
                result.AddError("PaymentPriority.Duplication", $"Provider payment priority cannot contains duplication of payment priority. {priorities}");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(UpdateProviderPaymentPriorityCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}