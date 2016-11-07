using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.UpdateApprenticeship
{
    public class UpdateApprenticeshipCommandValidator : IValidator<UpdateApprenticeshipCommand>
    {
        public ValidationResult Validate(UpdateApprenticeshipCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId <= 0)
                validationResult.AddError(nameof(item.AccountId), $"Specified Account is invalid.");

            if (item.Apprenticeship.CommitmentId <= 0)
                validationResult.AddError(nameof(item.Apprenticeship.CommitmentId), $"Specified Commitment is invalid.");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(UpdateApprenticeshipCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}