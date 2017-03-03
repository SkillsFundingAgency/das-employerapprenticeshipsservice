using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.ApproveApprenticeship
{
    public sealed class ApproveApprenticeshipCommandValidator : IValidator<ApproveApprenticeshipCommand>
    {
        public ValidationResult Validate(ApproveApprenticeshipCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var result = new ValidationResult();

            if (command.EmployerAccountId <= 0)
                result.AddError(nameof(command.EmployerAccountId), $"{nameof(command.EmployerAccountId)} has an invalid value.");

            if (command.CommitmentId <= 0)
                result.AddError(nameof(command.CommitmentId), $"{nameof(command.CommitmentId)} has an invalid value.");

            if (command.ApprenticeshipId <= 0)
                result.AddError(nameof(command.CommitmentId), $"{nameof(command.ApprenticeshipId)} has an invalid value.");

            if (string.IsNullOrEmpty(command.UserId))
                result.AddError(nameof(command.UserId), $"{nameof(command.UserId)} connot be null or empty.");

            return result;
        }

        public Task<ValidationResult> ValidateAsync(ApproveApprenticeshipCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
