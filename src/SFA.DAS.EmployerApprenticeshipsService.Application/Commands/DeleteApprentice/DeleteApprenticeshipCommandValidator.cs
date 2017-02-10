using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.DeleteApprentice
{
    public class DeleteApprenticeshipCommandValidator : IValidator<DeleteApprenticeshipCommand>
    {
        public ValidationResult Validate(DeleteApprenticeshipCommand item)
        {
            var result = new ValidationResult();

            if (item.ApprenticeshipId < 1)
            {
                result.AddError("ApprenticeshipId", "No ApprenticeshipId supplied");
            }

            if (item.AccountId < 1)
            {
                result.AddError("AccountId", "No AccountId supplied");
            }

            return result;
        }

        public Task<ValidationResult> ValidateAsync(DeleteApprenticeshipCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
