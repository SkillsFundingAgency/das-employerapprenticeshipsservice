using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData
{
    public class RefreshEmployerLevyDataCommandValidator : IValidator<RefreshEmployerLevyDataCommand>
    {
        public ValidationResult Validate(RefreshEmployerLevyDataCommand item)
        {
            //TODO VALIDATE!!!
            var validationResult = new ValidationResult();

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(RefreshEmployerLevyDataCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
