using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ChangeTeamMemberRole
{
    public class ChangeTeamMemberRoleCommandValidator : IValidator<ChangeTeamMemberRoleCommand>
    {
        public ValidationResult Validate(ChangeTeamMemberRoleCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedId))
                validationResult.AddError("HashedId", "No HashedId supplied");

            if (string.IsNullOrWhiteSpace(item.Email))
                validationResult.AddError("Email", "No Email supplied");

            if (item.RoleId == 0)
                validationResult.AddError("RoleId", "No RoleId supplied");

            if (string.IsNullOrWhiteSpace(item.ExternalUserId))
                validationResult.AddError("ExternalUserId", "No ExternalUserId supplied");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(ChangeTeamMemberRoleCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}