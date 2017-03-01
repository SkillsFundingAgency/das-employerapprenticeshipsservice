using System;
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

            if (string.IsNullOrEmpty(item.UserId))
                result.AddError(nameof(item.UserId), $"{nameof(item.UserId)} connot be null or empty.");

            return result;
        }

        public Task<ValidationResult> ValidateAsync(DeleteApprenticeshipCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
