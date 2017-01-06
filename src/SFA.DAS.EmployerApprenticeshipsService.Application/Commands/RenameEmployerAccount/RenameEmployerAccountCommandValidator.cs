using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.RenameEmployerAccount
{
    public class RenameEmployerAccountCommandValidator: IValidator<RenameEmployerAccountCommand>
    {
        public ValidationResult Validate(RenameEmployerAccountCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.NewName))
                validationResult.AddError("NewName", "New account name cannot be blank");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(RenameEmployerAccountCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
