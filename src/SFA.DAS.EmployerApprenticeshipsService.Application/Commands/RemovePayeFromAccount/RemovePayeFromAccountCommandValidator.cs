using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Commands.RemovePayeFromAccount
{
    public class RemovePayeFromAccountCommandValidator : IValidator<RemovePayeFromAccountCommand>
    {
        private readonly IMembershipRepository _membershipRepository;

        public RemovePayeFromAccountCommandValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(RemovePayeFromAccountCommand item)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(RemovePayeFromAccountCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
            }
            if (string.IsNullOrWhiteSpace(item.PayeRef))
            {
                validationResult.AddError(nameof(item.PayeRef), "PayeRef has not been supplied");
            }
            if (string.IsNullOrWhiteSpace(item.UserId))
            {
                validationResult.AddError(nameof(item.UserId), "UserId has not been supplied");
            }
            if (!item.RemoveScheme)
            {
                validationResult.AddError(nameof(item.RemoveScheme), "Please confirm you wish to remove the scheme");
            }

            if (validationResult.IsValid())
            {
                var member = await _membershipRepository.GetCaller(item.HashedAccountId, item.UserId);
                if (member== null || member.RoleId != (short) Role.Owner)
                {
                    validationResult.IsUnauthorized = true;
                }
            }

            return validationResult;
        }
    }
}