using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.RemoveTeamMember
{
    public class RemoveTeamMemberCommandValidator : IValidator<RemoveTeamMemberCommand>
    {
        public ValidationResult Validate(RemoveTeamMemberCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.UserId == 0)
                validationResult.AddError(nameof(item.UserId), "No UserId supplied");

            if (string.IsNullOrEmpty(item.HashedAccountId))
                validationResult.AddError(nameof(item.HashedAccountId), "No HashedAccountId supplied");

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