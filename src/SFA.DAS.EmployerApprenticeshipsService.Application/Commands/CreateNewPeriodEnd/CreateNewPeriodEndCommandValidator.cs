using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.CreateNewPeriodEnd
{
    public class CreateNewPeriodEndCommandValidator : IValidator<CreateNewPeriodEndCommand>
    {
        public ValidationResult Validate(CreateNewPeriodEndCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.NewPeriodEnd == null)
            {
                validationResult.AddError(nameof(item.NewPeriodEnd),"NewPeriodEnd has not been supplied");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(CreateNewPeriodEndCommand item)
        {
            throw new NotImplementedException();
        }
    }
}