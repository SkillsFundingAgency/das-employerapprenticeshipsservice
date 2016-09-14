using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RemovePayeFromAccount
{
    public class RemovePayeFromAccountCommandValidator : IValidator<RemovePayeFromAccountCommand>
    {
        public ValidationResult Validate(RemovePayeFromAccountCommand item)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValidationResult> ValidateAsync(RemovePayeFromAccountCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}