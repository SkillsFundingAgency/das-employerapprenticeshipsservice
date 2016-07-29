using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommandValidator : IValidator<UpsertRegisteredUserCommand>
    {
        public ValidationResult Validate(UpsertRegisteredUserCommand item)
        {
            var result = new ValidationResult();
            return result;
        }
    }
}
