using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.SubmitCommitment
{
    public sealed class SubmitCommitmentCommandValidator : IValidator<SubmitCommitmentCommand>
    {
        public ValidationResult Validate(SubmitCommitmentCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var result = new ValidationResult();

            if(command.EmployerAccountId <= 0)
                result.AddError(nameof(command.EmployerAccountId), $"{nameof(command.EmployerAccountId)} has an invalid value.");

            if (command.CommitmentId <= 0)
                result.AddError(nameof(command.CommitmentId), $"{nameof(command.CommitmentId)} has an invalid value.");

            if (string.IsNullOrWhiteSpace(command.UserDisplayName))
                result.AddError(nameof(command.UserDisplayName), $"{nameof(command.UserDisplayName)} must have a value.");

            if (string.IsNullOrWhiteSpace(command.UserEmailAddress))
                result.AddError(nameof(command.UserEmailAddress), $"{nameof(command.UserEmailAddress)} must have a value.");

            if (string.IsNullOrWhiteSpace(command.UserId))
                result.AddError(nameof(command.UserId), $"{nameof(command.UserId)} cannot be null or empty.");

            return result;
        }

        public Task<ValidationResult> ValidateAsync(SubmitCommitmentCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
