using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RefreshEmployerLevyData
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
