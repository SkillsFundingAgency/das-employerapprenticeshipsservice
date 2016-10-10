using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RemoveTeamMember
{
    public class RemoveTeamMemberCommandValidator : IValidator<RemoveTeamMemberCommand>
    {
        public ValidationResult Validate(RemoveTeamMemberCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.UserId == 0)
                validationResult.AddError(nameof(item.UserId), "No UserId supplied");

            if (string.IsNullOrEmpty(item.HashedId))
                validationResult.AddError(nameof(item.HashedId), "No HashedId supplied");

            if (string.IsNullOrWhiteSpace(item.ExternalUserId))
                validationResult.AddError(nameof(item.ExternalUserId), "No ExternalUserId supplied");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(RemoveTeamMemberCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}