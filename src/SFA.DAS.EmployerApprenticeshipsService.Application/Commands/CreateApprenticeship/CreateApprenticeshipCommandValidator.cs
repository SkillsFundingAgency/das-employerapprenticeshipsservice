using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateApprenticeship
{
    public sealed class CreateApprenticeshipCommandValidator : IValidator<CreateApprenticeshipCommand>
    {
        public ValidationResult Validate(CreateApprenticeshipCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId <= 0)
                validationResult.AddError(nameof(item.AccountId), $"Specified Account is invalid.");

            if (item.Apprenticeship.CommitmentId <= 0)
                validationResult.AddError(nameof(item.Apprenticeship.CommitmentId), $"Specified Commitment is invalid.");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(CreateApprenticeshipCommand item)
        {
            throw new NotImplementedException();
        }
    }
}