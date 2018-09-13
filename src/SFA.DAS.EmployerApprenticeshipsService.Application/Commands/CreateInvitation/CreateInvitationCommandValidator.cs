﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SFA.DAS.Authorization;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

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

            if (string.IsNullOrEmpty(item.HashedAccountId))
                validationResult.AddError(nameof(item.HashedAccountId), "No HashedAccountId supplied");

            if (string.IsNullOrWhiteSpace(item.EmailOfPersonBeingInvited))
                validationResult.AddError(nameof(item.EmailOfPersonBeingInvited), "Enter email address");
            else
            {
                if (!IsValidEmailFormat(item.EmailOfPersonBeingInvited))
                {
                    validationResult.AddError(nameof(item.EmailOfPersonBeingInvited), "Enter a valid email address");
                }
            }

            if (string.IsNullOrWhiteSpace(item.NameOfPersonBeingInvited))
                validationResult.AddError(nameof(item.NameOfPersonBeingInvited), "Enter name");

            if (item.RoleIdOfPersonBeingInvited == 0)
                validationResult.AddError(nameof(item.RoleIdOfPersonBeingInvited), "Select team member role");


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

                if (validationResult.IsValid() && caller != null)
                {
                    var existingTeamMember = await _membershipRepository.Get(caller.AccountId, item.EmailOfPersonBeingInvited);

                    if (existingTeamMember != null && existingTeamMember.IsUser)
                        validationResult.AddError(nameof(item.EmailOfPersonBeingInvited), $"{item.EmailOfPersonBeingInvited} is already invited");
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