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

            return result;
        }

        public Task<ValidationResult> ValidateAsync(SubmitCommitmentCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
