using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RemoveTeamMember
{
    public class RemoveTeamMemberCommandValidator : IValidator<RemoveTeamMemberCommand>
    {
        public ValidationResult Validate(RemoveTeamMemberCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.UserId == 0)
                validationResult.AddError("UserId", "No UserId supplied");

            if (item.AccountId == 0)
                validationResult.AddError("AccountId", "No AccountId supplied");

            if (string.IsNullOrWhiteSpace(item.ExternalUserId))
                validationResult.AddError("ExternalUserId", "No ExternalUserId supplied");

            return validationResult;
        }
    }
}