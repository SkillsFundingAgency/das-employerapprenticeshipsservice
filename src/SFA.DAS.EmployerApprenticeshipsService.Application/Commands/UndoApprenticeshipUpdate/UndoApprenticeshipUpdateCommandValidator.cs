using System.Threading.Tasks;

using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.UndoApprenticeshipUpdate
{
    public sealed class UndoApprenticeshipUpdateCommandValidator : IValidator<UndoApprenticeshipUpdateCommand>
    {
        public ValidationResult Validate(UndoApprenticeshipUpdateCommand command)
        {
            var result = new ValidationResult();

            if (command.ApprenticeshipId <= 0)
                result.AddError(nameof(command.ApprenticeshipId), $"{nameof(command.ApprenticeshipId)} has an invalid value.");

            if (command.AccountId <= 0)
                result.AddError(nameof(command.AccountId), $"{nameof(command.AccountId)} has an invalid value.");

            if (string.IsNullOrEmpty(command.UserId))
                result.AddError(nameof(command.UserId), $"{nameof(command.UserId)} has an invalid value.");

            return result;
        }

        public Task<ValidationResult> ValidateAsync(UndoApprenticeshipUpdateCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}