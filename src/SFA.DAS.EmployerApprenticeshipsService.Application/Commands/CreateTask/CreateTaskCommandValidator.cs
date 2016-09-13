using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateTask
{
    public sealed class CreateTaskCommandValidator : IValidator<CreateTaskCommand>
    {
        public ValidationResult Validate(CreateTaskCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var result = new ValidationResult();

            if (command.ProviderId<= 0)
                result.AddError(nameof(command.ProviderId), $"{nameof(command.ProviderId)} has an invalid value.");

            return result;
        }

        public Task<ValidationResult> ValidateAsync(CreateTaskCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
