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
            var validationResult = new ValidationResult();

            return validationResult;
        }
    }
}
