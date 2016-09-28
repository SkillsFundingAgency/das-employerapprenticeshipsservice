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
                validationResult.AddError(nameof(item.AccountId), $"{nameof(item.AccountId)} has an invalid value");

            if (item.Apprenticeship.CommitmentId <= 0)
                validationResult.AddError(nameof(item.Apprenticeship.CommitmentId), $"{nameof(item.Apprenticeship.CommitmentId)} has an invalid value");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(CreateApprenticeshipCommand item)
        {
            throw new NotImplementedException();
        }
    }
}