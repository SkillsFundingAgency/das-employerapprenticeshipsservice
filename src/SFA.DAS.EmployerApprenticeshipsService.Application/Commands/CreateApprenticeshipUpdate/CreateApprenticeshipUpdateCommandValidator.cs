using System.Threading.Tasks;

using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.CreateApprenticeshipUpdate
{
    public class CreateApprenticeshipUpdateCommandValidator : IValidator<CreateApprenticeshipUpdateCommand>
    {

        public ValidationResult Validate(CreateApprenticeshipUpdateCommand command)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(command.UserId))
                result.AddError(nameof(command.UserId), $"{nameof(command.UserId)} must have a value.");

            if (command.EmployerId <= 0)
                result.AddError(nameof(command.EmployerId), $"{nameof(command.EmployerId)} must have a value.");

            if (command.ApprenticeshipUpdate == null)
                result.AddError(nameof(command.ApprenticeshipUpdate), $"{nameof(command.ApprenticeshipUpdate)} must have a value.");

            if (command.ApprenticeshipUpdate != null && command.ApprenticeshipUpdate.ApprenticeshipId <= 0)
                result.AddError(nameof(command.ApprenticeshipUpdate.ApprenticeshipId), $"{nameof(command.ApprenticeshipUpdate.ApprenticeshipId)} must have a value.");

            return result;
        }

        public Task<ValidationResult> ValidateAsync(CreateApprenticeshipUpdateCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}