using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeToNewLegalEntity
{
    public class AddPayeToNewLegalEntityCommandValidator : IValidator<AddPayeToNewLegalEntityCommand>
    {
        public ValidationResult Validate(AddPayeToNewLegalEntityCommand item)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValidationResult> ValidateAsync(AddPayeToNewLegalEntityCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}