using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.DeleteInvitation
{
    public class DeleteInvitationCommandValidator : IValidator<DeleteInvitationCommand>
    {
        public ValidationResult Validate(DeleteInvitationCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.Id == 0)
                validationResult.AddError("Id", "No Id supplied");

            if (item.AccountId == 0)
                validationResult.AddError("AccountId", "No AccountId supplied");

            if (string.IsNullOrWhiteSpace(item.ExternalUserId))
                validationResult.AddError("ExternalUserId", "No ExternalUserId supplied");

            return validationResult;
        }
    }
}