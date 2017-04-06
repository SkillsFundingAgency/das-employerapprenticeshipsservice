using System.Threading.Tasks;

using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.ReviewApprenticeshipUpdate
{
    public sealed class ReviewApprenticeshipUpdateCommandValidator : IValidator<ReviewApprenticeshipUpdateCommand>
    {
        public ValidationResult Validate(ReviewApprenticeshipUpdateCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.ApprenticeshipId < 1)
            {
                validationResult.AddError(nameof(item.ApprenticeshipId), "ApprenticeshipId has not been defined");
            }

            if (item.AccountId < 1)
            {
                validationResult.AddError(nameof(item.AccountId), "AccountId has not been defined");
            }

            if (string.IsNullOrEmpty(item.UserId))
            {
                validationResult.AddError(nameof(item.UserId), "UserId has not been defined");
            }

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(ReviewApprenticeshipUpdateCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}
