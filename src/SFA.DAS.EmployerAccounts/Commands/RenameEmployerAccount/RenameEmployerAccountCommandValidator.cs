using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Validation;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount
{
    public class RenameEmployerAccountCommandValidator : IValidator<RenameEmployerAccountCommand>
    {
        private readonly IMembershipRepository _membershipRepository;

        public RenameEmployerAccountCommandValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(RenameEmployerAccountCommand item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(RenameEmployerAccountCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.NewName))
            {
                validationResult.AddError("NewName", "Enter a name");
            }

            if (validationResult.IsValid())
            {
                var caller = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);

                if (caller == null)
                {
                    validationResult.AddError("Membership", "User is not a member of this Account");
                    validationResult.IsUnauthorized = true;
                }
                else if (caller.Role != Role.Owner)
                {
                    validationResult.AddError("Membership", "User is not an Owner");
                    validationResult.IsUnauthorized = true;
                }
            }

            return validationResult;
        }
    }
}
