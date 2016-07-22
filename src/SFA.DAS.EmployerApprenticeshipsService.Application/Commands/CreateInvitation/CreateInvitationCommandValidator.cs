using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommandValidator : IValidator<CreateInvitationCommand>
    {
        public ValidationResult Validate(CreateInvitationCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId == 0)
                validationResult.AddError("AccountId", "No AccountId supplied");

            if (string.IsNullOrWhiteSpace(item.Email))
                validationResult.AddError("Email", "No Email supplied");

            if (string.IsNullOrWhiteSpace(item.Name))
                validationResult.AddError("Name", "No Name supplied");

            if (item.RoleId == 0)
                validationResult.AddError("RoleId", "No RoleId supplied");

            return validationResult;
        }
    }
}