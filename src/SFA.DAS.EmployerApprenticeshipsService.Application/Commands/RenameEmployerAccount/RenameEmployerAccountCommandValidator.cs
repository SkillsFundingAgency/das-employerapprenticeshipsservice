using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Commands.RenameEmployerAccount
{
    public class RenameEmployerAccountCommandValidator: IValidator<RenameEmployerAccountCommand>
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
                else if ((Role) caller.RoleId != Role.Owner)
                {
                    validationResult.AddError("Membership", "User is not an Owner");
                    validationResult.IsUnauthorized = true;
                }
            }

            return validationResult;
        }
    }
}
