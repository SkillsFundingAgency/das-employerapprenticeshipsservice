using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.PauseApprenticeship
{
    public sealed class PauseApprenticeshipCommandValidator : IValidator<PauseApprenticeshipCommand>
    {
        public ValidationResult Validate(PauseApprenticeshipCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var result = new ValidationResult();

            if (command.EmployerAccountId <= 0)
                result.AddError(nameof(command.EmployerAccountId), $"{nameof(command.EmployerAccountId)} has an invalid value.");

            if (command.CommitmentId <= 0)
                result.AddError(nameof(command.CommitmentId), $"{nameof(command.CommitmentId)} has an invalid value.");

            if (command.ApprenticeshipId <= 0)
                result.AddError(nameof(command.ApprenticeshipId), $"{nameof(command.ApprenticeshipId)} has an invalid value.");

            return result;
        }

        public Task<ValidationResult> ValidateAsync(PauseApprenticeshipCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
