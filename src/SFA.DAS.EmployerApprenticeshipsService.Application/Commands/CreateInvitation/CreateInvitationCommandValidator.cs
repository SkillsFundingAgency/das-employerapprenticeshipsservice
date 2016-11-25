using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommandValidator : IValidator<CreateInvitationCommand>
    {
        private readonly IMembershipRepository _membershipRepository;

        public CreateInvitationCommandValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
            
        }

        public ValidationResult Validate(CreateInvitationCommand item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(CreateInvitationCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.HashedId))
                validationResult.AddError(nameof(item.HashedId), "No HashedId supplied");

            if (string.IsNullOrWhiteSpace(item.Email))
                validationResult.AddError(nameof(item.Email), "Enter email address");
            else
            {
                if (!IsValidEmailFormat(item.Email))
                {
                    validationResult.AddError(nameof(item.Email), "Enter a valid email address");
                }
            }

            if (string.IsNullOrWhiteSpace(item.Name))
                validationResult.AddError(nameof(item.Name), "Enter name");

            if (item.RoleId == 0)
                validationResult.AddError(nameof(item.RoleId), "Select team member role");


            if (validationResult.IsValid())
            {
                var caller = await _membershipRepository.GetCaller(item.HashedId, item.ExternalUserId);

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

                if (validationResult.IsValid() && caller != null)
                {
                    var existingTeamMember = await _membershipRepository.Get(caller.AccountId, item.Email);

                    if (existingTeamMember != null && existingTeamMember.IsUser)
                        validationResult.AddError(nameof(item.Email), $"{item.Email} is already invited");
                }
           }

            return validationResult;
        }

        private bool IsValidEmailFormat(string email)
        {
            return Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
    }
}